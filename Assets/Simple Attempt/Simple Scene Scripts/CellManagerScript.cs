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
    PROPIGATE,
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

    /// <summary>
    /// The higher the number, the more likely each species is to populate a cell at the start of the level. This includes NONE for blank spaces.
    /// Note that this can be changed before the level generates to weight each scenario as you see fit.
    /// </summary>
    Dictionary<Species, int> speciesWeightDictionary;
    int emptySpaceWeight = 3;

    List<Rule> levelRules;
    /// <summary>
    /// Rules that only come into play for dying cells. These are referenced after the first round of rule application.
    /// </summary>
    List<Rule> deathRules;

    float updateRate = 1f;

    [SerializeField] Color deadColor = Color.grey;

    Species[] enabledSpecies;

    Dictionary<Coords, CellState> coordsToCellState;
    CellState[] cellStateArray;

    Dictionary<Coords, CellState> startingConditions;

    public void AssignLevel(Level level)
    {
        levelRules = new List<Rule>();
        deathRules = new List<Rule>();
        
        Rule[] rules = level.rules;

        for(int i = 0; i < rules.Length; i++)
        {
            Rule newRule = rules[i];

            if(newRule.deathRule)
            {
                deathRules.Add(newRule);
            }
            else
            {
                levelRules.Add(newRule);
            }
        }

        enabledSpecies = level.species;

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

    void SetSpecies(Coords coords, Species newSpecies)
    {
        CellState thisCellState = coordsToCellState[coords];
        thisCellState.species = newSpecies;

        if(thisCellState.alive)
        {
            gridManager.SetCellColor(coords, thisCellState.species.color);
        }        
    }    

    Species GetRandomSpecies()
    {
        return enabledSpecies[Random.Range(0, enabledSpecies.Length)];
    }

    void SetAlive(Coords coords, bool alive)
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
    }

    public void IncrementTime()
    {
        List<CellState> changingCells = new List<CellState>();
        Dictionary<Species, int> speciesPopulationDictionary = new Dictionary<Species, int>();

        for(int i = 0; i < enabledSpecies.Length; i++)
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

            Rule[] speciesLifeRules = null;

            if(cellState.species != null)
            {
                speciesLifeRules = cellState.species.otherRules;
            }

            if(speciesLifeRules != null)
            {
                for(int r = 0; r < speciesLifeRules.Length; r++)
                {
                    Rule currentRule = speciesLifeRules[r];

                    Result[] results = arbiter.TestRule(cellState.coords, currentRule);
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
                List<Result[]> successfulResults = new List<Result[]>();

                for(int s = 0; s < enabledSpecies.Length; s++)
                {
                    Rule[] propigationRules = enabledSpecies[s].propigationRules;

                    for(int p = 0; p < propigationRules.Length; p++)
                    {
                        Rule propigationRule = propigationRules[p];

                        if(propigationRule != null)
                        {
                            Result[] theseResults = arbiter.TestRule(cellState.coords, propigationRule);

                            if(theseResults != null)
                            {
                                successfulResults.Add(theseResults);
                            }
                        }
                    }
                    
                }

                if(successfulResults.Count > 0)
                {
                    ApplyResults(successfulResults[Random.Range(0, successfulResults.Count)]);
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

                if(result.newSpecies != null)
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

        //Loop through one final time to get accurate counts
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

    public int CountLivingNeighbors(Coords coords)
    {
        return CountLivingNeighbors(coords, null, null, null);
    }

    public int CountLivingNeighbors(Coords coords, List<Species> matchSpecies)
    {
        return CountLivingNeighbors(coords, matchSpecies, null, null);
    }

    public int CountLivingNeighbors(Coords coords, List<STATE> matchState)
    {
        return CountLivingNeighbors(coords, null, matchState, null);
    }

    public int CountLivingNeighbors(Coords coords, List<SPECIES_GROUP> matchSpeciesGroups)
    {
        return CountLivingNeighbors(coords, null, null, matchSpeciesGroups);
    }

    int CountLivingNeighbors(Coords coords, List<Species> matchSpecies, List<STATE> matchState, List<SPECIES_GROUP> matchSpeciesGroups)
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
                    if(matchSpecies.Contains(neighbor.species))
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

        return livingCount;
    }
}
