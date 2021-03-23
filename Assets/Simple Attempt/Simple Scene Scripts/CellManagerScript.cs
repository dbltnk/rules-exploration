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
    GOBLIN,    
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
    public SpeciesAttributes(string speciesName, SPECIES speciesEnum, Rule[] speciesRules, Color aliveColor)
    {
        this.speciesName = speciesName;
        this.aliveColor = aliveColor;
    }

    public string speciesName;
    public SPECIES speciesEnum;
    public Rule[] speciesRules;
    public Color aliveColor;    
}

public class CellManagerScript : MonoBehaviour
{
    [SerializeField] GridManagerScript gridManager = null;

    [SerializeField] ArbiterScript arbiter = null;

    [SerializeField] PROPIGATION_RULE propigationRule = PROPIGATION_RULE.CLASSIC_GENERIC;

    [SerializeField] List<Rule> levelRules = null;

    float updateRate = 1f;

    [SerializeField] Color deadColor = Color.grey;

    [SerializeField] List<SPECIES> enabledSpecies = new List<SPECIES>();

    [SerializeField] List<SpeciesAttributes> speciesAttributes = new List<SpeciesAttributes>();

    Dictionary<Coords, CellState> coordsToCellState;
    CellState[] cellStateArray;

    private void Awake()
    {
        if(propigationRule == PROPIGATION_RULE.CLASSIC_GENERIC)
        {
            levelRules.Add(new Rule(new Condition[] { new Condition(CONDITION_COMPARE_SOURCE.LIVING_NEIGHBOR_COUNT, CONDITION_TYPE.VALUE_WITHIN_RANGE, new Vector2Int(3, 3), null) }, 
                new Result[] { new Result(true, null, false, true, SPECIES.NONE, STATE.NORMAL) }));
        }
        else if(propigationRule == PROPIGATION_RULE.CLASSIC_RANDOM_SPECIES)
        {
            levelRules.Add(new Rule(new Condition[] { new Condition(CONDITION_COMPARE_SOURCE.LIVING_NEIGHBOR_COUNT, CONDITION_TYPE.VALUE_WITHIN_RANGE, new Vector2Int(3, 3), null) },
                new Result[] { new Result(true, null, false, true, SPECIES.NONE, STATE.NORMAL) }));
        }//DO THESE NEED TO BE TWO DIFFERENT CALLS????????????????????????????????????????????????????????????????????????????????????????????????

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
            SetSpecies(currentCoords, (SPECIES)Random.Range(1, 4));
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
        coordsToCellState[coords].species = speciesAttributes[(int)newSpecies];
    }

    SpeciesAttributes GetRandomSpecies()
    {
        SPECIES chosenSpecies = enabledSpecies[Random.Range(0, enabledSpecies.Count)];

        return speciesAttributes[(int)chosenSpecies];
    }

    void SetAlive(Coords coords, bool alive)
    {
        CellState thisCellState = coordsToCellState[coords];

        thisCellState.alive = alive;

        int speciesIndex = (int)thisCellState.species.speciesEnum;

        if(alive)
        {
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
                for(int t = 0; t < results.Length; t++)
                {
                    ApplyResult(results[t]);
                }
            }

            Rule[] speciesRules = cellState.species.speciesRules;
            for(int r = 0; r < speciesRules.Length; r++)
            {
                Result[] results = arbiter.TestRule(cellState.coords, speciesRules[r]);
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
                    List<NEIGHBORS> neighbors = result.affectedNeighbors;
                    for(int n = 0; n < neighbors.Count; n++)
                    {
                        Coords potentialNeighbor = gridManager.GetSpecificNeighbor(baseCoords, neighbors[n]);
                        if(potentialNeighbor.x < 0)
                        {
                            continue;
                        }
                        affectedCoords.Add(potentialNeighbor);
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

    public int CountLivingNeighbors(Coords coords, bool matchSpecies)
    {
        List<Coords> validNeighbors = gridManager.GetAllValidNeighbors(coords);
        int livingCount = 0;
        SPECIES sourceSpecies = coordsToCellState[coords].species.speciesEnum;

        for(int i = 0; i < validNeighbors.Count; i++)
        {
            CellState neighbor = coordsToCellState[validNeighbors[i]];

            if(neighbor.alive)
            {
                if(matchSpecies)
                {
                    if(neighbor.species.speciesEnum == sourceSpecies)
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

    SpeciesAttributes FindValidRegin(int neighborCount)
    {
        for(int i = 0; i < speciesAttributes.Count; i++)
        {
            SpeciesAttributes checkedSpecies = speciesAttributes[i];
            if(neighborCount >= checkedSpecies.reginRange.x && neighborCount <= checkedSpecies.reginRange.y)
            {
                if(enabledSpecies.Contains(checkedSpecies.speciesEnum))
                {
                    return checkedSpecies;
                }                
            }
        }

        return null;
    }
}
