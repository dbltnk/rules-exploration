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
    public Condition(CONDITION_COMPARE_SOURCE conditionCompareSource, CONDITION_TYPE conditionType, Vector2Int compareInts, List<Coords> compareCoords)
    {
        this.conditionCompareSource = conditionCompareSource;
        this.conditionType = conditionType;
        this.compareInts = compareInts;
        this.compareCoords = compareCoords;
        this.compareCoords = compareCoords;
    }

    public CONDITION_COMPARE_SOURCE conditionCompareSource;
    public CONDITION_TYPE conditionType;
    public Vector2Int compareInts;
    public List<Coords> compareCoords;
    public List<SPECIES> compareSpecies;
    public List<STATE> compareStates;
}

[System.Serializable]
public class Result
{
    public Result(bool affectSourceCoords, List<NEIGHBORS> affectedNeighbors, bool killIfAlive, bool reginIfDead, SPECIES newSpecies, STATE newState)
    {
        this.affectSourceCoords = affectSourceCoords;
        this.affectedNeighbors = affectedNeighbors;
        this.killIfAlive = killIfAlive;
        this.reginIfDead = reginIfDead;
        this.newSpecies = newSpecies;
        this.newState = newState;
    }

    public bool affectSourceCoords;
    public List<NEIGHBORS> affectedNeighbors;
    public bool killIfAlive;
    public bool reginIfDead;
    public SPECIES newSpecies;
    public STATE newState;
}

public enum CONDITION_COMPARE_SOURCE
{
    LIVING_NEIGHBOR_COUNT,
    LIVING_NEIGHBOR_MATCHING_SPECIES_COUNT,
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
}

public enum CONDITION_TYPE
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

            switch(thisCondition.conditionCompareSource)
            {
                case CONDITION_COMPARE_SOURCE.LIVING_NEIGHBOR_COUNT:
                    inputInt = cellManager.CountLivingNeighbors(coords, false);
                    break;
                case CONDITION_COMPARE_SOURCE.LIVING_NEIGHBOR_MATCHING_SPECIES_COUNT:
                    inputInt = cellManager.CountLivingNeighbors(coords, true);
                    break;
                case CONDITION_COMPARE_SOURCE.NEIGHBOR_DE:
                    AssignInputValuesBasedOnSpecificNeighbor(NEIGHBORS.DE);
                    break;
                case CONDITION_COMPARE_SOURCE.NEIGHBOR_DN:
                    AssignInputValuesBasedOnSpecificNeighbor(NEIGHBORS.DN);
                    break;
                case CONDITION_COMPARE_SOURCE.NEIGHBOR_DS:
                    AssignInputValuesBasedOnSpecificNeighbor(NEIGHBORS.DS);
                    break;
                case CONDITION_COMPARE_SOURCE.NEIGHBOR_DW:
                    AssignInputValuesBasedOnSpecificNeighbor(NEIGHBORS.DW);
                    break;
                case CONDITION_COMPARE_SOURCE.NEIGHBOR_NE:
                    AssignInputValuesBasedOnSpecificNeighbor(NEIGHBORS.NE);
                    break;
                case CONDITION_COMPARE_SOURCE.NEIGHBOR_NW:
                    AssignInputValuesBasedOnSpecificNeighbor(NEIGHBORS.NW);
                    break;
                case CONDITION_COMPARE_SOURCE.NEIGHBOR_SE:
                    AssignInputValuesBasedOnSpecificNeighbor(NEIGHBORS.SE);
                    break;
                case CONDITION_COMPARE_SOURCE.NEIGHBOR_SW:
                    AssignInputValuesBasedOnSpecificNeighbor(NEIGHBORS.SW);
                    break;
                case CONDITION_COMPARE_SOURCE.TARGET:
                    CellState cellState = cellManager.GetCellStateAtCoords(coords);
                    inputSpecies = cellState.species.speciesEnum;
                    inputState = cellState.state;
                    inputAlive = cellState.alive;
                    inputWallAdjacent = gridManager.CheckWallAdjacent(coords);
                    break;                    
            }          

            void AssignInputValuesBasedOnSpecificNeighbor(NEIGHBORS neighbor)
            {
                CellState cellState = cellManager.GetCellStateAtCoords(gridManager.GetSpecificNeighbor(coords, neighbor));
                if(cellState == null)
                {
                    return;
                }

                inputSpecies = cellState.species.speciesEnum;
                inputState = cellState.state;
                inputAlive = cellState.alive;
                inputWallAdjacent = gridManager.CheckWallAdjacent(cellState.coords);
            }

            switch(thisCondition.conditionType)
            {
                case CONDITION_TYPE.VALUE_OUTSIDE_RANGE:
                    if(inputInt >= thisCondition.compareInts.x &&
                        inputInt <= thisCondition.compareInts.y)
                    {
                        return null;
                    }
                    break;
                case CONDITION_TYPE.VALUE_WITHIN_RANGE:
                    if(inputInt < thisCondition.compareInts.x ||
                        inputInt > thisCondition.compareInts.y)
                    {
                        return null;
                    }
                    break;
                case CONDITION_TYPE.VALUE_MATCHES_SPECIES:
                    if(!thisCondition.compareSpecies.Contains(inputSpecies))
                    {
                        return null;
                    }
                    break;
                case CONDITION_TYPE.VALUE_DOES_NOT_MATCH_SPECIES:
                    if(thisCondition.compareSpecies.Contains(inputSpecies))
                    {
                        return null;
                    }
                    break;
                case CONDITION_TYPE.VALUE_ALIVE:
                    if(!inputAlive)
                    {
                        return null;
                    }
                    break;
                case CONDITION_TYPE.VALUE_DEAD:
                    if(inputAlive)
                    {
                        return null;
                    }
                    break;                
                case CONDITION_TYPE.VALUE_MATCHES_STATE:
                    if(!thisCondition.compareStates.Contains(inputState))
                    {
                        return null;
                    }
                    break;
                case CONDITION_TYPE.VALUE_DOES_NOT_MATCH_STATE:
                    if(thisCondition.compareStates.Contains(inputState))
                    {
                        return null;
                    }
                    break;
                case CONDITION_TYPE.VALUE_IS_WALL_ADJACENT:
                    break;
                default:
                    Debug.LogError("CONDITION_TYPE of current rule has not yet been set up in the ArbinterScript.");
                    return null;
            }

        }

        return rule.results;
    }
}
