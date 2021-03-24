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

public enum STATE
{
    NONE,
    NORMAL,
    SICKLY,
    HAPPY,
}

public enum SPECIES
{
    NONE,
    BLOB,
    FLOPPER,
    GOBLIN
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
}

[System.Serializable]
public class SpeciesAttributes
{
    public SpeciesAttributes(string speciesName, SPECIES speciesEnum, Rule[] speciesRules, Rule propigationRule, Color aliveColor)
    {
        this.speciesName = speciesName;
        this.aliveColor = aliveColor;
    }

    public string speciesName;
    public SPECIES speciesEnum;
    public Rule[] speciesRules;
    public Rule propigationRule;
    public Color aliveColor;    
}

public class CellManagerScript : MonoBehaviour
{
    [SerializeField] GridManagerScript gridManager = null;

    [SerializeField] ArbiterScript arbiter = null;

    [SerializeField] PROPIGATION_RULE propigationRule = PROPIGATION_RULE.CLASSIC_GENERIC;

    [SerializeField] bool enableSpeciesRules = true;

    [SerializeField] List<Rule> levelRules = null;

    float updateRate = 1f;

    [SerializeField] Color deadColor = Color.grey;

    [SerializeField] List<SPECIES> enabledSpecies = new List<SPECIES>();

    [SerializeField] List<SpeciesAttributes> speciesAttributes = new List<SpeciesAttributes>();

    Dictionary<Coords, CellState> coordsToCellState;
    CellState[] cellStateArray;

    private void Awake()
    {
        if(propigationRule != PROPIGATION_RULE.SPECIES_SPECIFIC)
        {
            levelRules.Add(new Rule(new Condition[] { new Condition(CONDITION_COMPARE_SOURCE.LIVING_NEIGHBOR_COUNT, CONDITION_TYPE.VALUE_WITHIN_RANGE, new Vector2Int(3, 3), null) }, 
                new Result[] { new Result(true, null, false, true, SPECIES.NONE, STATE.NORMAL) }));
        }

        StartConstantSimulate();
    }

    public void InitializeAllCells(Coords[] allCoords)
    {
        coordsToCellState = new Dictionary<Coords, CellState>();
        cellStateArray = new CellState[allCoords.Length];
        for(int i = 0; i < allCoords.Length; i++)
        {
            Coords currentCoords = allCoords[i];
            CellState newCellState = new CellState(currentCoords, STATE.NONE, null, false);
            coordsToCellState[currentCoords] = newCellState;
            cellStateArray[i] = newCellState;

            //Debug
            SetSpecies(currentCoords, enabledSpecies[Random.Range(0, enabledSpecies.Count)]);
            //SetSpecies(currentCoords, SPECIES.BLOB);
            SetAlive(currentCoords, Random.Range(0, 3) == 2);            
        }
    }

    public CellState GetCellStateAtCoords(Coords coods) { return coordsToCellState[coods]; }

    void SetState(Coords coords, STATE newState)
    {
        coordsToCellState[coords].state = newState;
    }

    void SetSpecies(Coords coords, SPECIES newSpecies)
    {
        //Debug.LogFormat("Setting species to {0} at {1}, {2}.", newSpecies, coords.x, coords.y);
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

        int speciesIndex = (int)thisCellState.species.speciesEnum;

        if(alive)
        {
            if(!wasAlreadyAlive)//Depending on rule set, bringing a cell back to life may require settting the species here.
            {
                if(propigationRule == PROPIGATION_RULE.CLASSIC_GENERIC)
                {
                    //Whatever the default species is should be set here.
                    thisCellState.species = speciesAttributes[(int)enabledSpecies[0]];//This is just the first thing in the enabled species list.
                }
                else if(propigationRule == PROPIGATION_RULE.CLASSIC_RANDOM_SPECIES)
                {
                    thisCellState.species = GetRandomSpecies();
                }
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
            SetAlive(cellStateArray[i].coords, false);
        }
    }

    public void AllCellsLiving()
    {
        for(int i = 0; i < cellStateArray.Length; i++)
        {
            CellState thisCellState = cellStateArray[i];

            if(thisCellState.species == null)
            {
                thisCellState.species = GetRandomSpecies();
            }

            SetAlive(cellStateArray[i].coords, true);
        }
    }

    public void IncrementTime()
    {
        List<CellState> changingCells = new List<CellState>();

        for(int i = 0; i < cellStateArray.Length; i++)
        {
            CellState cellState = cellStateArray[i];

            for(int r = 0; r < levelRules.Count; r++)
            {
                Result[] results = arbiter.TestRule(cellState.coords, levelRules[r]);
                if(results == null) { continue; }
                ApplyResults(results);
            }

            if(enableSpeciesRules)
            {
                Rule[] speciesRules = cellState.species.speciesRules;
                if(speciesRules == null) { continue; }
                for(int r = 0; r < speciesRules.Length; r++)
                {
                    ApplyResults(arbiter.TestRule(cellState.coords, speciesRules[r]));
                }
            }         
            
            if(!cellState.alive &&
                !cellState.futureAlive)
            {
                if(propigationRule == PROPIGATION_RULE.SPECIES_SPECIFIC)
                {
                    List<Result[]> successfulResults = new List<Result[]>();

                    for(int s = 0; s < enabledSpecies.Count; s++)
                    {
                        Result[] theseResults = arbiter.TestRule(cellState.coords, speciesAttributes[(int)enabledSpecies[s]].propigationRule);

                        if(theseResults != null)
                        {
                            //Debug.LogFormat("Adding succesful results for {0} at {1}, {2}.", enabledSpecies[s], cellState.coords.x, cellState.coords.y);
                            successfulResults.Add(theseResults);
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
                List<Coords> affectedCoords = new List<Coords>();
                Coords baseCoords = cellState.coords;
                if(result.affectSourceCoords)
                {
                    affectedCoords.Add(baseCoords);
                }
                List<NEIGHBORS> neighbors = result.affectedNeighbors;
                if(neighbors != null)
                {
                    for(int n = 0; n < neighbors.Count; n++)
                    {
                        Coords potentialNeighbor = gridManager.GetSpecificNeighbor(baseCoords, neighbors[n]);
                        if(potentialNeighbor.x < 0)
                        {
                            continue;
                        }
                        affectedCoords.Add(potentialNeighbor);
                    }
                }                    

                for(int c = 0; c < affectedCoords.Count; c++)
                {
                    Coords currentAffectedCoords = affectedCoords[c];
                    CellState currentAffectedCellState = coordsToCellState[currentAffectedCoords];

                    if(result.killIfAlive && currentAffectedCellState.alive)
                    {
                        currentAffectedCellState.futureAlive = false;
                    }
                    if(result.reginIfDead && !currentAffectedCellState.alive)
                    {
                        currentAffectedCellState.futureAlive = true;
                    }
                    if(result.newSpecies != SPECIES.NONE)
                    {
                        //Debug.LogFormat("Newspecies is {0} at {1}, {2}..", result.newSpecies, currentAffectedCoords.x, currentAffectedCoords.y);
                        currentAffectedCellState.futureSpecies = result.newSpecies;
                    }
                    if(result.newState != STATE.NONE)
                    {
                        currentAffectedCellState.futureState = result.newState;
                    }

                    AddToChangingCells(currentAffectedCellState);
                }
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

    public int CountLivingNeighbors(Coords coords, List<SPECIES> matchSpecies)
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
                else
                {
                    livingCount++;
                }                
            }
        }

        return livingCount;
    }
}
