using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Rule
{
    public Rule(Condition[] conditions, Result[] results)
    {
        this.conditions = conditions;
        this.results = results;
    }

    public Condition[] conditions;
    public Result[] results;
}

[System.Serializable]
public class Condition
{
    public Condition(SOURCE source, CONDITON condition, Vector2Int compareInts, List<Coords> compareCoords, List<SPECIES> compareSpecies, List<STATE> compareStates)
    {
        this.source = source;
        this.condition = condition;
        this.compareInts = compareInts;
        this.compareCoords = compareCoords;
        this.compareCoords = compareCoords;
        this.compareSpecies = compareSpecies;
        this.compareStates = compareStates;
    }


    public Condition(SOURCE source, CONDITON condition, Vector2Int compareInts)
    {
        this.source = source;
        this.condition = condition;
        this.compareInts = compareInts;
        compareCoords = null;
        compareSpecies = null;
        compareStates = null;
    }

    public Condition(SOURCE source, CONDITON condition, Vector2Int compareInts, STATE compareState)
    {
        this.source = source;
        this.condition = condition;
        this.compareInts = compareInts;
        compareStates = new List<STATE> { compareState };
        compareCoords = null;
        compareSpecies = null;
    }

    public Condition(SOURCE source, CONDITON condition, Vector2Int compareInts, SPECIES compareSpecies)
    {
        this.source = source;
        this.condition = condition;
        this.compareInts = compareInts;
        this.compareSpecies = new List<SPECIES> { compareSpecies };
        compareCoords = null;
        compareStates = null;
    }

    public Condition(SOURCE source, CONDITON condition, STATE compareState)
    {
        this.source = source;
        this.condition = condition;
        compareStates = new List<STATE> { compareState };
        compareCoords = null;
        compareSpecies = null;
    }

    public Condition(SOURCE source, CONDITON condition, SPECIES compareSpecies)
    {
        this.source = source;
        this.condition = condition;
        this.compareSpecies = new List<SPECIES> { compareSpecies };
        compareCoords = null;
        compareStates = null;
    }

    public Condition(SOURCE source, CONDITON condition)
    {
        this.source = source;
        this.condition = condition;
        compareCoords = null;
        compareSpecies = null;
        compareStates = null;
    }

    public SOURCE source;
    public CONDITON condition;
    public Vector2Int compareInts;
    public List<Coords> compareCoords;
    public List<SPECIES> compareSpecies;
    public List<STATE> compareStates;
}

[System.Serializable]
public class Result
{
    public Result(LIFE_EFFECT lifeEffect, SPECIES newSpecies, STATE newState)
    {
        this.lifeEffect = lifeEffect;
        this.newSpecies = newSpecies;
        this.newState = newState;
    }

    public Result(LIFE_EFFECT lifeEffect, SPECIES newSpecies)
    {
        this.lifeEffect = lifeEffect;
        this.newSpecies = newSpecies;
    }

    public Result(LIFE_EFFECT lifeEffect, STATE newState)
    {
        this.lifeEffect = lifeEffect;
        this.newState = newState;
    }

    public Result(SPECIES newSpecies, STATE newState)
    {
        this.newSpecies = newSpecies;
        this.newState = newState;
    }

    public Result(LIFE_EFFECT lifeEffect)
    {
        this.lifeEffect = lifeEffect;
    }

    public Result(SPECIES newSpecies)
    {
        this.newSpecies = newSpecies;
    }

    public Result(STATE newState)
    {
        this.newState = newState;
    }

    public LIFE_EFFECT lifeEffect;
    public SPECIES newSpecies;
    public STATE newState;
}

public enum SOURCE
{
    LIVING_NEIGHBOR_COUNT,
    LIVING_NEIGHBOR_MATCHING_SPECIES_COUNT,
    LIVING_NEIGHBORS_MATCHING_STATE_COUNT,
    /// <summary>
    /// The cell currently being reviewed by the arbiter.
    /// </summary>
    TARGET,
    NEIGHBOR_NW,
    NEIGHBOR_DN,
    NEIGHBOR_NE,
    NEIGHBOR_DE,
    NEIGHBOR_SE,
    NEIGHBOR_DS,
    NEIGHBOR_SW,
    NEIGHBOR_DW,
    RANDOM_D6,//Random roll of 1-6
}

public enum CONDITON
{
    /// <summary>
    /// Range defined as compareInts.x to compareInts.y, inclusive.
    /// </summary>
    VALUE_WITHIN_RANGE,
    /// <summary>
    /// Range defined as compareInts.x to compareInts.y, inclusive.
    /// </summary>
    VALUE_OUTSIDE_RANGE,
    VALUE_MATCHES_SPECIES,
    VALUE_DOES_NOT_MATCH_SPECIES,
    VALUE_MATCHES_STATE,
    VALUE_DOES_NOT_MATCH_STATE,
    VALUE_ALIVE,
    VALUE_DEAD,
    VALUE_IS_WALL_ADJACENT,
    /// <summary>
    /// This step, the cell is going to be set to dead.
    /// </summary>
    CELL_IS_DYING,
}

public class ArbiterScript : MonoBehaviour
{
    [SerializeField] CellManagerScript cellManager = null;
    [SerializeField] GridManagerScript gridManager = null;

    public Result[] TestRule(Coords coords, Rule rule)
    {
        Condition[] conditions = rule.conditions;

        for(int i = 0; i < conditions.Length; i++)
        {
            Condition thisCondition = conditions[i];

            int inputInt = 0;
            bool inputAlive = false;
            SPECIES inputSpecies = SPECIES.NONE;
            STATE inputState = STATE.NONE;
            bool inputWallAdjacent = false;
            bool inputIsDying = false;

            switch(thisCondition.source)
            {
                case SOURCE.LIVING_NEIGHBOR_COUNT:
                    inputInt = cellManager.CountLivingNeighbors(coords);
                    break;
                case SOURCE.LIVING_NEIGHBOR_MATCHING_SPECIES_COUNT:
                    inputInt = cellManager.CountLivingNeighbors(coords, thisCondition.compareSpecies);
                    break;
                case SOURCE.LIVING_NEIGHBORS_MATCHING_STATE_COUNT:
                    inputInt = cellManager.CountLivingNeighbors(coords, thisCondition.compareStates);
                    break;
                case SOURCE.NEIGHBOR_DE:
                    AssignInputValuesBasedOnSpecificNeighbor(NEIGHBORS.DE);
                    break;
                case SOURCE.NEIGHBOR_DN:
                    AssignInputValuesBasedOnSpecificNeighbor(NEIGHBORS.DN);
                    break;
                case SOURCE.NEIGHBOR_DS:
                    AssignInputValuesBasedOnSpecificNeighbor(NEIGHBORS.DS);
                    break;
                case SOURCE.NEIGHBOR_DW:
                    AssignInputValuesBasedOnSpecificNeighbor(NEIGHBORS.DW);
                    break;
                case SOURCE.NEIGHBOR_NE:
                    AssignInputValuesBasedOnSpecificNeighbor(NEIGHBORS.NE);
                    break;
                case SOURCE.NEIGHBOR_NW:
                    AssignInputValuesBasedOnSpecificNeighbor(NEIGHBORS.NW);
                    break;
                case SOURCE.NEIGHBOR_SE:
                    AssignInputValuesBasedOnSpecificNeighbor(NEIGHBORS.SE);
                    break;
                case SOURCE.NEIGHBOR_SW:
                    AssignInputValuesBasedOnSpecificNeighbor(NEIGHBORS.SW);
                    break;
                case SOURCE.TARGET:
                    CellState cellState = cellManager.GetCellStateAtCoords(coords);
                    if(cellState.species == null)
                    {
                        inputSpecies = SPECIES.NONE;
                    }
                    else
                    {
                        inputSpecies = cellState.species.speciesEnum;
                    }                    
                    inputState = cellState.state;
                    inputAlive = cellState.alive;
                    inputWallAdjacent = gridManager.CheckWallAdjacent(coords);
                    inputIsDying = CheckIsDying(cellState);
                    break;
                case SOURCE.RANDOM_D6:
                    inputInt = Random.Range(1, 7);
                    break;
            }          

            bool CheckIsDying(CellState cellState)
            { 
                if(cellState.alive && !cellState.futureAlive)
                {
                    return true;
                }

                return false;
            }

            void AssignInputValuesBasedOnSpecificNeighbor(NEIGHBORS neighbor)
            {
                Coords thisNeighborCoords = gridManager.GetSpecificNeighbor(coords, neighbor);

                if(!gridManager.CheckValidCell(thisNeighborCoords))
                {
                    return;
                }

                CellState cellState = cellManager.GetCellStateAtCoords(thisNeighborCoords);
                if(cellState == null)
                {
                    return;
                }

                if(cellState.species == null)
                {
                    inputSpecies = SPECIES.NONE;
                }
                else
                {
                    inputSpecies = cellState.species.speciesEnum;
                }
                
                inputState = cellState.state;
                inputAlive = cellState.alive;
                inputWallAdjacent = gridManager.CheckWallAdjacent(cellState.coords);
                inputIsDying = CheckIsDying(cellState);
            }

            switch(thisCondition.condition)
            {
                case CONDITON.VALUE_OUTSIDE_RANGE:
                    if(inputInt >= thisCondition.compareInts.x &&
                        inputInt <= thisCondition.compareInts.y)
                    {
                        return null;
                    }
                    break;
                case CONDITON.VALUE_WITHIN_RANGE:
                    if(inputInt < thisCondition.compareInts.x ||
                        inputInt > thisCondition.compareInts.y)
                    {
                        return null;
                    }
                    break;
                case CONDITON.VALUE_MATCHES_SPECIES:
                    if(!inputAlive) { return null; }
                    if(thisCondition.compareSpecies == null)
                    {
                        return null;
                    }
                    if(!thisCondition.compareSpecies.Contains(inputSpecies))
                    {
                        return null;
                    }
                    break;
                case CONDITON.VALUE_DOES_NOT_MATCH_SPECIES:
                    if(thisCondition.compareSpecies == null)
                    {
                        return null;
                    }
                    if(thisCondition.compareSpecies.Contains(inputSpecies))
                    {
                        return null;
                    }
                    break;
                case CONDITON.VALUE_ALIVE:
                    if(!inputAlive)
                    {
                        return null;
                    }
                    break;
                case CONDITON.VALUE_DEAD:
                    if(inputAlive)
                    {
                        return null;
                    }
                    break;                
                case CONDITON.VALUE_MATCHES_STATE:
                    if(!inputAlive) { return null; }
                    if(!thisCondition.compareStates.Contains(inputState))
                    {
                        return null;
                    }
                    break;
                case CONDITON.VALUE_DOES_NOT_MATCH_STATE:
                    if(thisCondition.compareStates.Contains(inputState))
                    {
                        return null;
                    }
                    break;
                case CONDITON.VALUE_IS_WALL_ADJACENT:
                    if(!inputWallAdjacent)
                    {
                        return null;
                    }
                    break;
                case CONDITON.CELL_IS_DYING:
                    if(!inputIsDying)
                    {
                        return null;
                    }
                    break;
                default:
                    Debug.LogError("CONDITION_TYPE of current rule has not yet been set up in the ArbinterScript.");
                    return null;
            }
        }

        return rule.results;
    }
}
