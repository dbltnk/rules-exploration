using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CONDITION
{
    NONE,
    NORMAL,
    SICKLY,
    HAPPY,
}

public enum SPECIES
{
    BLOB,
    FLOPPER,
    GOBLIN,
    NONE//This must always be the last on this list so as to not throw off the int value.
}

public enum RULES
{
    STANDARD,
    SERIOUS,
    WACKY,
    SOMETHING_ELSE,
}

public class CellState
{
    public CellState(Coords coords, CONDITION condition, SpeciesAttributes species, bool alive)
    {
        this.coords = coords;
        this.condition = condition;
        this.species = species;
        this.alive = alive;

        futureCondition = CONDITION.NONE;
        futureSpecies = SPECIES.NONE;
        futureAlive = alive;
    }

    public Coords coords;
    public CONDITION condition;
    public CONDITION futureCondition;
    public SpeciesAttributes species;
    public SPECIES futureSpecies;
    public bool alive;
    public bool futureAlive;
}

[System.Serializable]
public class SpeciesAttributes
{
    public SpeciesAttributes(string speciesName, SPECIES speciesEnum, Vector2Int neighborRange, Vector2Int reginRange, Color aliveColor)
    {
        this.speciesName = speciesName;
        this.neighborRange = neighborRange;
        this.reginRange = reginRange;
        this.aliveColor = aliveColor;
    }

    public string speciesName;
    public SPECIES speciesEnum;
    public Vector2Int neighborRange;
    public Vector2Int reginRange;
    public Color aliveColor;
}

public class CellManagerScript : MonoBehaviour
{
    [SerializeField] GridManagerScript gridManager = null;

    [SerializeField] RULES rules;

    [SerializeField] float updateRate = 2f;

    [SerializeField] Color deadColor = Color.grey;

    [SerializeField] List<SPECIES> enabledSpecies = new List<SPECIES>();

    [SerializeField] List<SpeciesAttributes> speciesAttributes = new List<SpeciesAttributes>();

    Dictionary<Coords, CellState> coordsToCellState;
    CellState[] cellStateArray;

    private void Awake()
    {
        StartConstantSimulate();
    }

    public void InitializeAllCells(Coords[] allCoords)
    {
        coordsToCellState = new Dictionary<Coords, CellState>();
        cellStateArray = new CellState[allCoords.Length];
        for(int i = 0; i < allCoords.Length; i++)
        {
            Coords currentCoords = allCoords[i];
            CellState newCellState = new CellState(currentCoords, CONDITION.NONE, null, false);
            coordsToCellState[currentCoords] = newCellState;
            cellStateArray[i] = newCellState;

            //Debug
            SetSpecies(currentCoords, (SPECIES)Random.Range(0, 3));
            //SetSpecies(currentCoords, SPECIES.BLOB);
            SetAlive(currentCoords, Random.Range(0, 3) == 2);            
        }
    }

    void SetCondition(Coords coords, CONDITION newCondition)
    {
        coordsToCellState[coords].condition = newCondition;
    }

    void SetSpecies(Coords coords, SPECIES newSpecies)
    {
        coordsToCellState[coords].species = speciesAttributes[(int)newSpecies];
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

    public void IncrementTime()
    {
        List<CellState> changingCells = new List<CellState>();

        for(int i = 0; i < cellStateArray.Length; i++)
        {
            CellState cellState = cellStateArray[i];

            switch(rules)
            {
                case RULES.STANDARD:
                    int livingCount = CountLivingNeighbors(cellState.coords);

                    bool currentlyAlive = cellState.alive;

                    SpeciesAttributes currentSpecies = cellState.species;

                    if(currentlyAlive)
                    {
                        if(livingCount < currentSpecies.neighborRange.x || livingCount > currentSpecies.neighborRange.y)
                        {
                            AddToChangingCells(cellState);
                            cellState.futureAlive = false;
                        }
                    }
                    else
                    {
                        SpeciesAttributes newRegin = FindValidRegin(livingCount);

                        if(newRegin != null)
                        {
                            AddToChangingCells(cellState);
                            cellState.futureSpecies = newRegin.speciesEnum;
                            cellState.futureAlive = true;
                        }
                    }
                    break;
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

            CONDITION newCondition = changingState.futureCondition;
            if(newCondition != CONDITION.NONE)
            {
                SetCondition(changingState.coords, newCondition);
                changingState.futureCondition = CONDITION.NONE;
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

    public int CountLivingNeighbors(Coords coords)
    {
        List<Coords> validNeighbors = gridManager.GetAllValidNeighbors(coords);
        int livingCount = 0;

        for(int i = 0; i < validNeighbors.Count; i++)
        {
            if(coordsToCellState[validNeighbors[i]].alive)
            {
                livingCount++;
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
