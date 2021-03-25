using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PROPIGATION_RULE
{
    /// <summary>
    /// Level-wide rule will generate a generic living cell.
    /// </summary>
    CLASSIC_GENERIC,
    /// <summary>
    /// Level-wide rule will generate a random species from the enabled species list.
    /// </summary>
    CLASSIC_RANDOM_SPECIES,
    /// <summary>
    /// Each species will use their specific propigation rule.
    /// </summary>
    SPECIES_SPECIFIC,
}

public enum RULESET
{
    CUSTOM,
    STANDARD, 
    SCRAMBLE_RULES,
    SCRAMBLE_SPECIES,
    SCRAMBLE_ALL,
    ALL_RULES,
}

public enum STATE
{
    NONE,
    NORMAL,
    SICKLY,
    HAPPY,
}

public enum LIFE_EFFECT
{
    NONE,
    PROPIGATE,
    KILL,
}

public class CellState
{
    public CellState(Coords coords, STATE state, SpeciesAttributes species, bool alive)
    {
        this.coords = coords;
        this.state = state;
        this.species = species;
        this.alive = alive;

        futureState = STATE.NONE;
        futureSpecies = SPECIES.NONE;
        futureAlive = alive;
    }

    public Coords coords;
    public STATE state;
    public STATE futureState;
    public SpeciesAttributes species;
    public SPECIES futureSpecies;
    public bool alive;
    public bool futureAlive;

    public CellState Copy()
    {
        return new CellState(coords, state, species, alive);
    }
}

public class CellManagerScript : MonoBehaviour
{
    [SerializeField] GridManagerScript gridManager = null;

    [SerializeField] ArbiterScript arbiter = null;

    [SerializeField] RULESET levelRuleset = RULESET.CUSTOM;

    [SerializeField] bool enableSpeciesRules = true;

    [SerializeField] bool randomizeSpeciesOfDeadCellsOnDishRefill = true;

    [SerializeField] LayerStatusScript layerStatus = null;

    /// <summary>
    /// The higher the number, the more likely each species is to populate a cell at the start of the level. This includes NONE for blank spaces.
    /// Note that this can be changed before the level generates to weight each scenario as you see fit.
    /// </summary>
    Dictionary<SPECIES, int> speciesWeightDictionary = new Dictionary<SPECIES, int>
    {
        { SPECIES.NONE, 15 },
        { SPECIES.BLOB, 5 },
        { SPECIES.FLOPPER, 3 },
        { SPECIES.GOBLIN, 2 },
        { SPECIES.ROCK, 1 },
    };

    List<Rule> levelRules;
    /// <summary>
    /// Rules that only come into play for dying cells. These are referenced after the first round of rule application.
    /// </summary>
    List<Rule> deathRules;

    float updateRate = 1f;

    [SerializeField] Color deadColor = Color.grey;

    [SerializeField] List<SPECIES> enabledSpecies = new List<SPECIES>();

    List<SpeciesAttributes> speciesAttributes = new List<SpeciesAttributes>();

    [SerializeField] List<SpeciesAttributes> speciesOverride = null;

    Dictionary<Coords, CellState> coordsToCellState;
    CellState[] cellStateArray;

    Dictionary<Coords, CellState> startingConditions;

    private void Awake()
    {
        levelRules = new List<Rule>();
        deathRules = new List<Rule>();

        speciesAttributes.AddRange(SpeciesReference.defaultSpeciesAttributes);

        if(speciesOverride != null)
        {
            for(int i = 0; i < speciesOverride.Count; i++)
            {
                SpeciesAttributes currentOverride = speciesOverride[i];
                speciesAttributes[(int)currentOverride.speciesEnum] = currentOverride;
            }
        }

        switch(levelRuleset)
        {
            case RULESET.STANDARD:
                levelRules.Add(GetPremadeRule(RULE_REFERENCE.STANDARD_DEATH));
                levelRules.Add(GetPremadeRule(RULE_REFERENCE.STANDARD_PROPIGATION));
                enabledSpecies = new List<SPECIES> { SPECIES.BLOB };
                enableSpeciesRules = false;
                break;
            case RULESET.CUSTOM:
                deathRules.Add(RuleReference.premadeDeathRules[(int)DEATH_RULES.ROCKS_ARE_IMMORTAL]);
                levelRules.Add(GetPremadeRule(RULE_REFERENCE.FLOPPERS_CAN_BECOME_BLOBS));
                levelRules.Add(GetPremadeRule(RULE_REFERENCE.SICK_CELLS_CAN_DIE));
                enableSpeciesRules = true;
                break;
            case RULESET.SCRAMBLE_RULES:
                ApplyScrambledRules();
                break;
            case RULESET.SCRAMBLE_ALL:
                ApplyScrambledRules();
                break;
            case RULESET.ALL_RULES:
                for(int i = 2; i < (int)RULE_REFERENCE.FINAL_RULE_LEAVE_EMPTY; i++)//We do ignore the first two standard rules.
                {
                    RULE_REFERENCE currentRule = (RULE_REFERENCE)i;

                    levelRules.Add(GetPremadeRule(currentRule));
                }
                for(int i = 0; i < (int)DEATH_RULES.FINAL_RULE_LEAVE_EMPTY; i++)
                {
                    deathRules.Add(RuleReference.premadeDeathRules[i]);
                }
                break;
        }        
    }

    void ApplyScrambledRules()
    {
        for(int i = 2; i < (int)RULE_REFERENCE.FINAL_RULE_LEAVE_EMPTY; i++)//ignoring the first two standard rules.
        {
            if(Random.Range(0, 2) == 1)
            {
                levelRules.Add(RuleReference.premadeRuleArray[i]);
            }
        }

        for(int i = 0; i < (int)DEATH_RULES.FINAL_RULE_LEAVE_EMPTY; i++)
        {
            if(Random.Range(0, 2) == 1)
            {
                deathRules.Add(RuleReference.premadeDeathRules[i]);
            }
        }
    }

    Rule GetPremadeRule(RULE_REFERENCE ruleReference)
    {
        return RuleReference.premadeRuleArray[(int)ruleReference];
    }

    Dictionary<SPECIES, int> initalPopulationDictionary;
    List<PopulationCount> initialPopulationCount;

    public void InitializeAllCells(Coords[] allCoords)
    {
        coordsToCellState = new Dictionary<Coords, CellState>();
        startingConditions = new Dictionary<Coords, CellState>();
        cellStateArray = new CellState[allCoords.Length];

        List<SPECIES> speciesPool = new List<SPECIES>();

        initalPopulationDictionary = new Dictionary<SPECIES, int>();

        for(int i = 0; i < enabledSpecies.Count; i++)
        {
            initalPopulationDictionary[enabledSpecies[i]] = 0;
        }

        for(int i = 0; i < speciesWeightDictionary[SPECIES.NONE]; i++)
        {
            speciesPool.Add(SPECIES.NONE);
        }

        for(int i = 0; i < enabledSpecies.Count; i++)
        {
            SPECIES thisSpecies = enabledSpecies[i];            

            for(int s = 0; s < speciesWeightDictionary[thisSpecies]; s++)
            {
                speciesPool.Add(thisSpecies);
            }
        }

        for(int i = 0; i < allCoords.Length; i++)
        {
            Coords currentCoords = allCoords[i];
            CellState newCellState = new CellState(currentCoords, STATE.NONE, null, false);
            coordsToCellState[currentCoords] = newCellState;
            cellStateArray[i] = newCellState;

            SPECIES thisSpecies = speciesPool[Random.Range(0, speciesPool.Count)];

            SetSpecies(currentCoords, thisSpecies);

            if(thisSpecies == SPECIES.NONE)
            {
                SetAlive(currentCoords, false);
            }
            else
            {                
                SetAlive(currentCoords, true);
                initalPopulationDictionary[thisSpecies]++;
            }

            startingConditions[currentCoords] = newCellState.Copy();
        }

        initialPopulationCount = new List<PopulationCount>();

        foreach(SPECIES species in initalPopulationDictionary.Keys)
        {
            initialPopulationCount.Add(new PopulationCount(species, initalPopulationDictionary[species]));
        }

        layerStatus.UpdateSpeciesPopulation(initialPopulationCount, initalPopulationDictionary);

        StartConstantSimulate();
    }

    public void ReInitializeAllCells()
    {
        Coords[] allCoords = gridManager.GetAllCoords();

        for(int i = 0; i < allCoords.Length; i++)
        {
            Coords currentCoords = allCoords[i];
            CellState cellState = coordsToCellState[currentCoords];
            CellState initialCellState = startingConditions[currentCoords];

            SetSpecies(currentCoords, initialCellState.species.speciesEnum);
            SetState(currentCoords, initialCellState.state);
            SetAlive(currentCoords, initialCellState.alive);

            cellState.futureSpecies = SPECIES.NONE;
            cellState.futureState = STATE.NONE;
            cellState.futureAlive = cellState.alive;
        }

        layerStatus.UpdateSpeciesPopulation(initialPopulationCount, initalPopulationDictionary);
    }

    public CellState GetCellStateAtCoords(Coords coods) { return coordsToCellState[coods]; }

    void SetState(Coords coords, STATE newState)
    {
        coordsToCellState[coords].state = newState;
    }

    void SetSpecies(Coords coords, SPECIES newSpecies)
    {
        CellState thisCellState = coordsToCellState[coords];
        thisCellState.species = speciesAttributes[(int)newSpecies];

        if(thisCellState.alive)
        {
            gridManager.SetCellColor(coords, thisCellState.species.aliveColor);
        }        
    }    

    SpeciesAttributes GetRandomSpecies()
    {
        SPECIES chosenSpecies = enabledSpecies[Random.Range(0, enabledSpecies.Count)];

        return speciesAttributes[(int)chosenSpecies];
    }

    void SetAlive(Coords coords, bool alive)
    {
        CellState thisCellState = coordsToCellState[coords];

        bool wasAlreadyAlive = thisCellState.alive;

        thisCellState.alive = alive;
        
        if(alive)
        {
            if(!wasAlreadyAlive && !enableSpeciesRules)//Depending on rule set, bringing a cell back to life may require settting the species here.
            {
                thisCellState.species = speciesAttributes[(int)enabledSpecies[0]];//This is just the first thing in the enabled species list.
            }

            gridManager.SetCellColor(coords, thisCellState.species.aliveColor);
        }
        else
        {
            gridManager.SetCellColor(coords, deadColor);
        }
    }

    public void ClearAllLife()
    {
        for(int i = 0; i < cellStateArray.Length; i ++)
        {
            CellState cellState = cellStateArray[i];
            Coords coords = cellState.coords;

            cellState.futureSpecies = SPECIES.NONE;
            cellState.futureState = STATE.NONE;
            cellState.futureAlive = false;

            SetSpecies(coords, SPECIES.NONE);
            SetState(coords, STATE.NONE);
            SetAlive(coords, false);
        }
    }

    public void AllCellsLiving()
    {
        for(int i = 0; i < cellStateArray.Length; i++)
        {
            CellState thisCellState = cellStateArray[i];

            if(thisCellState.species == null ||
                (!thisCellState.alive && randomizeSpeciesOfDeadCellsOnDishRefill))
            {
                thisCellState.species = GetRandomSpecies();
            }

            SetAlive(cellStateArray[i].coords, true);
        }
    }

    public void IncrementTime()
    {
        List<CellState> changingCells = new List<CellState>();
        Dictionary<SPECIES, int> speciesPopulationDictionary = new Dictionary<SPECIES, int>();

        for(int i = 0; i < enabledSpecies.Count; i++)
        {
            speciesPopulationDictionary[enabledSpecies[i]] = 0;
        }

        for(int i = 0; i < cellStateArray.Length; i++)
        {
            CellState cellState = cellStateArray[i];

            for(int r = 0; r < levelRules.Count; r++)
            {
                Result[] results = arbiter.TestRule(cellState.coords, levelRules[r]);
                if(results != null)
                {
                    ApplyResults(results);
                }
            }

            if(enableSpeciesRules)
            {
                Rule speciesLifeRules = null;

                if(cellState.species != null)
                {
                    speciesLifeRules = cellState.species.lifeRule;
                }

                if(speciesLifeRules != null)
                {
                    Result[] results = arbiter.TestRule(cellState.coords, speciesLifeRules);
                    if(results != null)
                    {
                        ApplyResults(results);
                    }
                }
            }

            for(int r = 0; r < deathRules.Count; r++)
            {
                Result[] results = arbiter.TestRule(cellState.coords, deathRules[r]);
                if(results != null)
                {
                    ApplyResults(results);
                }
            }

            if(!cellState.alive &&
                !cellState.futureAlive)
            {
                if(enableSpeciesRules)
                {
                    List<Result[]> successfulResults = new List<Result[]>();

                    for(int s = 0; s < enabledSpecies.Count; s++)
                    {
                        Rule propigationRule = speciesAttributes[(int)enabledSpecies[s]].propigationRule;

                        if(propigationRule != null)
                        {
                            Result[] theseResults = arbiter.TestRule(cellState.coords, propigationRule);

                            if(theseResults != null)
                            {
                                successfulResults.Add(theseResults);
                            }
                        }
                    }

                    if(successfulResults.Count > 0)
                    {
                        ApplyResults(successfulResults[Random.Range(0, successfulResults.Count)]);
                    }
                }
            }

            void ApplyResults(Result[] results)
            {
                for(int t = 0; t < results.Length; t++)
                {
                    ApplyResult(results[t]);
                }
            }

            void ApplyResult(Result result)
            {
                Coords affectedCoords = cellState.coords;

                CellState affectedCellState = coordsToCellState[affectedCoords];

                switch(result.lifeEffect)
                {
                    case LIFE_EFFECT.KILL:
                        affectedCellState.futureAlive = false;
                        break;
                    case LIFE_EFFECT.PROPIGATE:
                        affectedCellState.futureAlive = true;
                        break;
                }

                if(result.newSpecies != SPECIES.NONE)
                {
                    affectedCellState.futureSpecies = result.newSpecies;
                }
                if(result.newState != STATE.NONE)
                {
                    affectedCellState.futureState = result.newState;
                }

                AddToChangingCells(affectedCellState);
            }
        }

        void AddToChangingCells(CellState cellState)
        {
            if(changingCells.Contains(cellState))
            {
                return;
            }

            changingCells.Add(cellState);
        }

        for(int i = 0; i < changingCells.Count; i++)
        {
            CellState changingState = changingCells[i];

            STATE newState = changingState.futureState;
            if(newState != STATE.NONE)
            {
                SetState(changingState.coords, newState);
                changingState.futureState = STATE.NONE;
            }

            SPECIES newSpecies = changingState.futureSpecies;
            if(newSpecies != SPECIES.NONE)
            {
                SetSpecies(changingState.coords, newSpecies);
                changingState.futureSpecies = SPECIES.NONE;
            }

            bool newAlive = changingState.futureAlive;
            if(newAlive != changingState.alive)
            {
                SetAlive(changingState.coords, newAlive);
            }
        }

        //Loop through one final time to get accurate counts
        for(int i = 0; i < cellStateArray.Length; i++)
        {
            CellState thisCellState = cellStateArray[i];

            if(thisCellState.species == null)
            {
                continue;
            }

            SPECIES thisSpecies = thisCellState.species.speciesEnum;

            if(thisSpecies == SPECIES.NONE)
            {
                continue;
            }

            if(!thisCellState.alive)
            {
                continue;
            }

            speciesPopulationDictionary[thisSpecies]++;
        }

        List<PopulationCount> populationCountList = new List<PopulationCount>();

        foreach(SPECIES species in speciesPopulationDictionary.Keys)
        {
            populationCountList.Add(new PopulationCount(species, speciesPopulationDictionary[species]));
        }

        layerStatus.UpdateSpeciesPopulation(populationCountList, speciesPopulationDictionary);
    }

    public void SetSimulationSpeed(float stepsPerSecond)
    {
        updateRate = 1f / stepsPerSecond;
    }

    public void StartConstantSimulate()
    {
        StopCoroutine("RunSimulation");
        StartCoroutine("RunSimulation");
    }

    public void StopConstantSimulate()
    {
        StopCoroutine("RunSimulation");
    }

    IEnumerator RunSimulation()
    {
        yield return new WaitForSeconds(updateRate);
        IncrementTime();
        StartCoroutine("RunSimulation");
    }

    public int CountLivingNeighbors(Coords coords)
    {
        return CountLivingNeighbors(coords, null, null);
    }

    public int CountLivingNeighbors(Coords coords, List<SPECIES> matchSpecies)
    {
        return CountLivingNeighbors(coords, matchSpecies, null);
    }

    public int CountLivingNeighbors(Coords coords, List<STATE> matchState)
    {
        return CountLivingNeighbors(coords, null, matchState);
    }

    int CountLivingNeighbors(Coords coords, List<SPECIES> matchSpecies, List<STATE> matchState)
    {
        List<Coords> validNeighbors = gridManager.GetAllValidNeighbors(coords);

        int livingCount = 0;

        for(int i = 0; i < validNeighbors.Count; i++)
        {
            CellState neighbor = coordsToCellState[validNeighbors[i]];

            if(neighbor.alive)
            {
                if(matchSpecies != null)
                {
                    if(matchSpecies.Contains(neighbor.species.speciesEnum))
                    {
                        livingCount++;
                    }
                }
                else if(matchState != null)
                {
                    if(matchState.Contains(neighbor.state))
                    {
                        livingCount++;
                    }
                }
                else
                {
                    livingCount++;
                }                
            }
        }

        return livingCount;
    }
}
