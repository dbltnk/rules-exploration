using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    PROPAGATE,
    KILL,
}

public class CellState
{
    public CellState(Coords coords, STATE state, Species species, bool alive)
    {
        this.coords = coords;
        this.state = state;
        this.species = species;
        this.alive = alive;

        futureState = STATE.NONE;
        futureSpecies = null;
        futureAlive = alive;
    }

    public Coords coords;
    public STATE state;
    public STATE futureState;
    public Species species;
    public Species futureSpecies;
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

    [SerializeField] LayerStatusScript layerStatus = null;

    Dictionary<Coords, CellObjectScript> coordsToCellManager = new Dictionary<Coords, CellObjectScript>();

    /// <summary>
    /// The higher the number, the more likely each species is to populate a cell at the start of the level. This includes NONE for blank spaces.
    /// Note that this can be changed before the level generates to weight each scenario as you see fit.
    /// </summary>
    Dictionary<Species, int> speciesWeightDictionary;
    int emptySpaceWeight = 3;

    List<Rule> levelRules;

    float updateRate = 1f;

    [SerializeField] Color deadColor = Color.grey;

    Species[] enabledSpecies;

    Dictionary<Coords, CellState> coordsToCellState;
    CellState[] cellStateArray;

    Dictionary<Coords, CellState> startingConditions;

    public void AssignLevel(Level level, GameManagerScript gameManager, SpeciesBank speciesBank)
    {
        layerStatus.AssignSpeciesBank(speciesBank);

        levelRules = new List<Rule>();
        
        RuleObject[] ruleObjects = level.ruleObjects;

        RulesBank rulesBank = gameManager.GetComponent<RulesBank>();

        for(int i = 0; i < ruleObjects.Length; i++)
        {
            levelRules.Add(rulesBank.GetRuleFromRuleObjectAtRuntime(ruleObjects[i], null));
        }

        SpeciesObject[] specificSpecies = level.specificSpecies;

        Species[] specificSpeciesConverted = new Species[specificSpecies.Length];

        for(int i = 0; i < specificSpecies.Length; i++)
        {
            specificSpeciesConverted[i] = new Species(specificSpecies[i], rulesBank);
        }

        speciesBank.AddSpecies(specificSpeciesConverted);

        int speciesAmount = specificSpecies.Length;

        int procgenSpeciesAmount = Random.Range(level.procgenSpeciesAmount.x, level.procgenSpeciesAmount.y + 1);

        speciesAmount += procgenSpeciesAmount;

        enabledSpecies = new Species[speciesAmount];

        List<string> usedNames = new List<string>();

        for(int i = 0; i < specificSpeciesConverted.Length; i++)
        {
            enabledSpecies[i] = specificSpeciesConverted[i];
            usedNames.Add(level.specificSpecies[i].defaultName);
        }

        int amountOfOldSpeciesToUse = 0;
        int amountOfNewSpeciesToGenerate = 0;

        for(int i = 0; i < procgenSpeciesAmount; i++)
        {
            if(Random.Range(0, 8) == 0)
            {
                amountOfNewSpeciesToGenerate++;
            }
            else
            {
                amountOfOldSpeciesToUse++;
            }
        }

        List<Species> returningSpecies = speciesBank.GetKnownSpecies(amountOfOldSpeciesToUse, usedNames);

        int speciesIndex = level.specificSpecies.Length;

        for(int i = 0; i < returningSpecies.Count; i++)
        {
            enabledSpecies[speciesIndex] = returningSpecies[i];
            amountOfOldSpeciesToUse--;
            speciesIndex++;
        }

        amountOfNewSpeciesToGenerate += amountOfOldSpeciesToUse;//Because the returning species list may come up short, we add what's left back into the count of new species we want to generate.

        Species[] newSpeciesArray = speciesBank.GenerateNewSpecies(amountOfNewSpeciesToGenerate);

        for(int i = 0; i < amountOfNewSpeciesToGenerate; i++)
        {
            enabledSpecies[speciesIndex] = newSpeciesArray[i];
            speciesIndex++;
        }

        gameManager.SaveGame();

        speciesWeightDictionary = new Dictionary<Species, int>();

        for(int i = 0; i < enabledSpecies.Length; i++)
        {
            Species thisSpecies = enabledSpecies[i];
            switch(thisSpecies.startingPopulation)
            {
                case SPECIES_STARTING_POPULATION.VERY_RARE:
                    speciesWeightDictionary[thisSpecies] = 1;
                    break;
                case SPECIES_STARTING_POPULATION.RARE:
                    speciesWeightDictionary[thisSpecies] = 2;
                    break;
                case SPECIES_STARTING_POPULATION.UNCOMMON:
                    speciesWeightDictionary[thisSpecies] = 3;
                    break;
                case SPECIES_STARTING_POPULATION.COMMON:
                    speciesWeightDictionary[thisSpecies] = 4;
                    break;
                case SPECIES_STARTING_POPULATION.VERY_COMMON:
                    speciesWeightDictionary[thisSpecies] = 5;
                    break;
                case SPECIES_STARTING_POPULATION.UBIQUITOUS:
                    speciesWeightDictionary[thisSpecies] = 6;
                    break;
            }
        }

        emptySpaceWeight = enabledSpecies.Length * 4;
    }

    Dictionary<Species, int> initalPopulationDictionary;
    List<PopulationCount> initialPopulationCount;

    public void InitializeAllCells(Coords[] allCoords)
    {
        coordsToCellState = new Dictionary<Coords, CellState>();
        startingConditions = new Dictionary<Coords, CellState>();
        cellStateArray = new CellState[allCoords.Length];

        List<Species> speciesPool = new List<Species>();

        initalPopulationDictionary = new Dictionary<Species, int>();

        for(int i = 0; i < enabledSpecies.Length; i++)
        {
            initalPopulationDictionary[enabledSpecies[i]] = 0;
        }

        for(int i = 0; i < emptySpaceWeight; i++)
        {
            speciesPool.Add(null);
        }

        for(int i = 0; i < enabledSpecies.Length; i++)
        {
            Species thisSpecies = enabledSpecies[i];            

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

            Species thisSpecies = speciesPool[Random.Range(0, speciesPool.Count)];

            SetSpecies(currentCoords, thisSpecies);

            if(thisSpecies == null)
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

        foreach(Species species in initalPopulationDictionary.Keys)
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

            SetSpecies(currentCoords, initialCellState.species);
            SetState(currentCoords, initialCellState.state);
            SetAlive(currentCoords, initialCellState.alive);

            cellState.futureSpecies = null;
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

    public Species GetSpecies(Coords coords) { return coordsToCellState[coords].species; }

    void SetSpecies(Coords coords, Species newSpecies)
    {
        CellState thisCellState = coordsToCellState[coords];
        thisCellState.species = newSpecies;

        if(thisCellState.species == null)
        {
            thisCellState.alive = false;
        }

        if(thisCellState.alive)
        {
            gridManager.SetCellColor(coords, thisCellState.species.color);
        }        
    }

    Species GetRandomSpecies()
    {
        return enabledSpecies[Random.Range(0, enabledSpecies.Length)];
    }    

    public void CopyCellStateOntoNewCell(Coords source, Coords destination)
    {
        CellState souceState = coordsToCellState[source];

        SetSpecies(destination, souceState.species);
        SetState(destination, souceState.state);
        SetAlive(destination, souceState.alive);
    }

    public bool IsAlive(Coords coords) { return coordsToCellState[coords].alive; }

    public void SetAlive(Coords coords, bool alive)
    {
        CellState thisCellState = coordsToCellState[coords];

        bool wasAlreadyAlive = thisCellState.alive;

        thisCellState.alive = alive;
        
        if(alive)
        {
            gridManager.SetCellColor(coords, thisCellState.species.color);
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

            cellState.futureSpecies = null;
            cellState.futureState = STATE.NONE;
            cellState.futureAlive = false;

            SetSpecies(coords, null);
            SetState(coords, STATE.NONE);
            SetAlive(coords, false);
        }

        CountAndReportPopulation();
    }

    public void AllCellsLiving()
    {
        for(int i = 0; i < cellStateArray.Length; i++)
        {
            CellState thisCellState = cellStateArray[i];

            if(thisCellState.species == null ||
                !thisCellState.alive)
            {
                thisCellState.species = GetRandomSpecies();
            }

            SetAlive(cellStateArray[i].coords, true);
        }

        CountAndReportPopulation();
    }

    public struct BirthResult
    {
        public BirthResult(Species species, Result result)
        {
            this.species = species;
            this.result = result;
        }

        public Species species;
        public Result result;
    }

    public void IncrementTime()
    {
        List<CellState> changingCells = new List<CellState>();        

        for(int i = 0; i < cellStateArray.Length; i++)
        {
            CellState cellState = cellStateArray[i];

            for(int r = 0; r < levelRules.Count; r++)
            {
                Result result = arbiter.TestRule(cellState.coords, levelRules[r]);
                if(result != null)
                {
                    ApplySimpleResult(result);
                }
            }

            Rule speciesDeathRule = null;
            Rule[] otherRules = null;

            bool applyDeathRule = false;

            if(cellState.species != null)
            {                
                speciesDeathRule = cellState.species.deathRule;
                applyDeathRule = !speciesDeathRule.nullRule;
                otherRules = cellState.species.otherRules;
            }

            if(applyDeathRule)
            {
                Result result = arbiter.TestRule(cellState.coords, speciesDeathRule);
                if(result != null)
                {
                    ApplySimpleResult(result);
                }
            }

            //Debug.Log(otherRules);

            if(otherRules != null)
            {
                for(int r = 0; r < otherRules.Length; r++)
                {
                    Result result = arbiter.TestRule(cellState.coords, otherRules[r]);
                    if(result != null)
                    {
                        ApplySimpleResult(result);
                    }
                }
            }            

            if(!cellState.alive &&
                !cellState.futureAlive)
            {
                List<BirthResult> successfulResults = new List<BirthResult>();

                for(int s = 0; s < enabledSpecies.Length; s++)
                {
                    Rule birthRule = enabledSpecies[s].birthRule;

                    if(!birthRule.nullRule)
                    {
                        Result thisResult = arbiter.TestRule(cellState.coords, birthRule);

                        if(thisResult != null)
                        {
                            successfulResults.Add(new BirthResult(enabledSpecies[s], thisResult));
                        }
                    }
                }

                if(successfulResults.Count > 0)
                {
                    ApplyBirthResult(successfulResults[Random.Range(0, successfulResults.Count)]);
                }
            }

            void ApplyBirthResult(BirthResult results)
            {
                ApplyResult(results.result, results.species);
            }

            void ApplySimpleResult(Result result)
            {
                ApplyResult(result, null);
            }

            void ApplyResult(Result result, Species newSpecies)
            {
                Coords affectedCoords = cellState.coords;

                CellState affectedCellState = coordsToCellState[affectedCoords];

                if(newSpecies != null)
                {
                    affectedCellState.futureSpecies = newSpecies;
                }

                switch(result.lifeEffect)
                {
                    case LIFE_EFFECT.KILL:
                        affectedCellState.futureAlive = false;
                        break;
                    case LIFE_EFFECT.PROPAGATE:
                        affectedCellState.futureAlive = true;
                        break;
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

            Species newSpecies = changingState.futureSpecies;
            if(newSpecies != null)
            {
                SetSpecies(changingState.coords, newSpecies);
                changingState.futureSpecies = null;
            }

            bool newAlive = changingState.futureAlive;
            if(newAlive != changingState.alive)
            {
                SetAlive(changingState.coords, newAlive);
            }
        }

        CountAndReportPopulation();
    }

    void CountAndReportPopulation()
    {
        Dictionary<Species, int> speciesPopulationDictionary = new Dictionary<Species, int>();

        for(int i = 0; i < enabledSpecies.Length; i++)
        {
            speciesPopulationDictionary[enabledSpecies[i]] = 0;
        }

        for(int i = 0; i < cellStateArray.Length; i++)
        {
            CellState thisCellState = cellStateArray[i];

            if(thisCellState.species == null)
            {
                continue;
            }

            Species thisSpecies = thisCellState.species;

            if(thisSpecies == null)
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

        foreach(Species species in speciesPopulationDictionary.Keys)
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

    public int CountLivingNeighbors(Coords coords, NEIGHBOR_STYLE neighborStyle, bool wallsAreAlive)
    {
        return CountLivingNeighbors(coords, neighborStyle, wallsAreAlive, null, null, null);
    }

    public int CountLivingNeighbors(Coords coords, NEIGHBOR_STYLE neighborStyle, bool wallsAreAlive, Species matchSpecies)
    {
        return CountLivingNeighbors(coords, neighborStyle, wallsAreAlive, matchSpecies, null, null);
    }

    public int CountLivingNeighbors(Coords coords, NEIGHBOR_STYLE neighborStyle, bool wallsAreAlive, List<STATE> matchState)
    {
        return CountLivingNeighbors(coords, neighborStyle, wallsAreAlive, null, matchState, null);
    }

    public int CountLivingNeighbors(Coords coords, NEIGHBOR_STYLE neighborStyle, bool wallsAreAlive, List<SPECIES_GROUP> matchSpeciesGroups)
    {
        return CountLivingNeighbors(coords, neighborStyle, wallsAreAlive, null, null, matchSpeciesGroups);
    }

    int CountLivingNeighbors(Coords coords, NEIGHBOR_STYLE neighborStyle, bool wallsAreAlive,  Species matchSpecies, List<STATE> matchState, List<SPECIES_GROUP> matchSpeciesGroups)
    {
        List<Coords> validNeighbors = gridManager.GetAllValidNeighbors(coords, neighborStyle);

        int livingCount = 0;

        for(int i = 0; i < validNeighbors.Count; i++)
        {
            CellState neighbor = coordsToCellState[validNeighbors[i]];

            if(neighbor.alive)
            {
                if(matchSpecies != null)
                {
                    if(neighbor.species.defaultName == matchSpecies.defaultName)
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
                else if(matchSpeciesGroups != null)
                {
                    Species neighborSpecies = neighbor.species;

                    if(neighborSpecies != null)
                    {
                        bool matchFound = false;

                        for(int s = 0; s < neighborSpecies.speciesGroups.Count; s++)
                        {
                            for(int subS = 0; subS < matchSpeciesGroups.Count; subS++)
                            {
                                if(neighborSpecies.speciesGroups.Contains(matchSpeciesGroups[subS]))
                                {
                                    livingCount++;
                                    matchFound = true;
                                    break;
                                }
                            }          
                            
                            if(matchFound) { break; }
                        }                        
                    }                    
                }
                else
                {
                    livingCount++;
                }                
            }
        }

        // If we always treat walls as alive then we need to increase the living count by the number of walls.
        if (wallsAreAlive) {
            // TODO(azacherl): Replace this 8 when we support more than the Moore neighborhood.
            int numberOfWalls = 8 - validNeighbors.Count;
            livingCount += numberOfWalls;
        }

        return livingCount;
    }
}
