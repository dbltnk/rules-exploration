using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Condition
{
    public Condition(SOURCE source, CONDITON_PARAMETER conditionParameter, Vector2Int compareInts, List<SPECIES_GROUP> compareSpeciesGroups, List<STATE> compareStates)
    {
        this.source = source;
        this.conditionParameter = conditionParameter;
        this.compareInts = compareInts;
        this.compareSpeciesGroups = compareSpeciesGroups;
        this.compareStates = compareStates;
    }

    public Condition(SOURCE source, CONDITON_PARAMETER conditionParameter, Vector2Int compareInts, List<SPECIES_GROUP> compareSpeciesGroups)
    {
        this.source = source;
        this.conditionParameter = conditionParameter;
        this.compareInts = compareInts;
        this.compareSpeciesGroups = compareSpeciesGroups;
        compareStates = new List<STATE>();
    }

    public Condition(SOURCE source, CONDITON_PARAMETER conditionParameter, Vector2Int compareInts)
    {
        this.source = source;
        this.conditionParameter = conditionParameter;
        this.compareInts = compareInts;
        compareSpeciesGroups = new List<SPECIES_GROUP>();
        compareStates = new List<STATE>();
    }

    public Condition(SOURCE source, CONDITON_PARAMETER conditionParameter, Vector2Int compareInts, STATE compareState)
    {
        this.source = source;
        this.conditionParameter = conditionParameter;
        this.compareInts = compareInts;
        compareStates = new List<STATE> { compareState };
        compareSpeciesGroups = new List<SPECIES_GROUP>();
    }

    public Condition(SOURCE source, CONDITON_PARAMETER conditionParameter, Vector2Int compareInts, SPECIES_GROUP compareSpeciesGroups)
    {
        this.source = source;
        this.conditionParameter = conditionParameter;
        this.compareInts = compareInts;
        this.compareSpeciesGroups = new List<SPECIES_GROUP> { compareSpeciesGroups };
        compareStates = new List<STATE>();
    }

    public Condition(SOURCE source, CONDITON_PARAMETER conditionParameter, STATE compareState)
    {
        this.source = source;
        this.conditionParameter = conditionParameter;
        compareStates = new List<STATE> { compareState };
        compareSpeciesGroups = new List<SPECIES_GROUP>();
    }

    public Condition(SOURCE source, CONDITON_PARAMETER conditionParameter, SPECIES_GROUP compareSpeciesGroups)
    {
        this.source = source;
        this.conditionParameter = conditionParameter;
        this.compareSpeciesGroups = new List<SPECIES_GROUP> { compareSpeciesGroups };
        compareStates = new List<STATE>();
    }

    public Condition(SOURCE source, CONDITON_PARAMETER conditionParameter)
    {
        this.source = source;
        this.conditionParameter = conditionParameter;
        compareSpeciesGroups = new List<SPECIES_GROUP>();
        compareStates = new List<STATE>();
    }

    public SOURCE source;
    public CONDITON_PARAMETER conditionParameter;
    public Vector2Int compareInts;
    public List<SPECIES_GROUP> compareSpeciesGroups;
    public List<STATE> compareStates;

    public Condition Copy()
    {
        return new Condition(source, conditionParameter, compareInts, compareSpeciesGroups, compareStates);
    }
}

[System.Serializable]
public class Result
{
    public Result(LIFE_EFFECT lifeEffect, STATE newState)
    {
        this.lifeEffect = lifeEffect;
        this.newState = newState;
    }

    public Result(LIFE_EFFECT lifeEffect)
    {
        this.lifeEffect = lifeEffect;
    }

    public Result(STATE newState)
    {
        this.newState = newState;
    }

    public LIFE_EFFECT lifeEffect;
    public STATE newState;
}

public enum SOURCE
{
    LIVING_NEIGHBOR_COUNT,
    LIVING_NEIGHBOR_MATCHING_SPECIES_COUNT,
    LIVING_NEIGHBOR_MATCHING_SPECIES_GROUP_COUNT,
    LIVING_NEIGHBORS_MATCHING_STATE_COUNT,
    /// <summary>
    /// The cell currently being reviewed by the arbiter.
    /// </summary>
    SOURCE_CELL,
    NEIGHBOR_NW,
    NEIGHBOR_DN,
    NEIGHBOR_NE,
    NEIGHBOR_DE,
    NEIGHBOR_SE,
    NEIGHBOR_DS,
    NEIGHBOR_SW,
    NEIGHBOR_DW,
    RANDOM_D6,//Random roll of 1-6
    TOP_SPECIES,//species with the highest population.
}

public enum CONDITON_PARAMETER
{
    /// <summary>
    /// Range defined as compareInts.x to compareInts.y, inclusive.
    /// </summary>
    VALUE_WITHIN_RANGE,
    /// <summary>
    /// Range defined as compareInts.x to compareInts.y, inclusive.
    /// </summary>
    VALUE_OUTSIDE_RANGE,
    MATCHES_SPECIES,
    DOES_NOT_MATCH_SPECIES,
    CONTAINS_SPECIES_GROUP,
    DOES_NOT_CONTAIN_SPECIES_GROUP,
    MATCHES_STATE,
    DOES_NOT_MATCH_STATE,
    IS_ALIVE,
    IS_DEAD,
    IS_WALL_ADJACENT,
    /// <summary>
    /// This step, the cell is going to be set to dead.
    /// </summary>
    IS_DYING,
}

public class ArbiterScript : MonoBehaviour
{
    [SerializeField] CellManagerScript cellManager = null;
    [SerializeField] GridManagerScript gridManager = null;
    [SerializeField] LayerStatusScript layerManager = null;

    public Result TestRule(Coords coords, Rule rule)
    {
        Condition[] conditions = rule.conditions;

        for(int i = 0; i < conditions.Length; i++)
        {
            Condition thisCondition = conditions[i];

            int inputInt = 0;
            bool inputAlive = false;
            Species inputSpecies = null;
            List<SPECIES_GROUP> inputSpeciesGroups = null;
            STATE inputState = STATE.NONE;
            bool inputWallAdjacent = false;
            bool inputIsDying = false;

            switch(thisCondition.source)
            {
                case SOURCE.LIVING_NEIGHBOR_COUNT:
                    inputInt = cellManager.CountLivingNeighbors(coords, rule.neighborStyle, rule.wallsAreAlive);
                    break;
                case SOURCE.LIVING_NEIGHBOR_MATCHING_SPECIES_COUNT:
                    inputInt = cellManager.CountLivingNeighbors(coords, rule.neighborStyle, rule.wallsAreAlive, cellManager.GetSpecies(coords));
                    break;
                case SOURCE.LIVING_NEIGHBOR_MATCHING_SPECIES_GROUP_COUNT:
                    inputInt = cellManager.CountLivingNeighbors(coords, rule.neighborStyle, rule.wallsAreAlive, thisCondition.compareSpeciesGroups);
                    break;
                case SOURCE.LIVING_NEIGHBORS_MATCHING_STATE_COUNT:
                    inputInt = cellManager.CountLivingNeighbors(coords, rule.neighborStyle, rule.wallsAreAlive, thisCondition.compareStates);
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
                case SOURCE.SOURCE_CELL:
                    CellState cellState = cellManager.GetCellStateAtCoords(coords);
                    inputSpecies = cellState.species;
                    if(inputSpecies != null)
                    {
                        inputSpeciesGroups = cellState.species.speciesGroups;
                    }                    
                    inputState = cellState.state;
                    inputAlive = cellState.alive;
                    inputWallAdjacent = gridManager.CheckWallAdjacent(coords);
                    inputIsDying = CheckIsDying(cellState);
                    break;
                case SOURCE.RANDOM_D6:
                    inputInt = Random.Range(1, 7);
                    break;
                case SOURCE.TOP_SPECIES:
                    inputSpecies = layerManager.GetSpeciesAtRank(0);
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

                inputSpecies = cellState.species;

                if(inputSpecies == null)
                {
                    inputSpeciesGroups = null;
                }
                else
                {
                    inputSpeciesGroups = inputSpecies.speciesGroups;
                }
                
                inputState = cellState.state;
                inputAlive = cellState.alive;
                inputWallAdjacent = gridManager.CheckWallAdjacent(cellState.coords);
                inputIsDying = CheckIsDying(cellState);
            }

            switch(thisCondition.conditionParameter)
            {
                case CONDITON_PARAMETER.VALUE_OUTSIDE_RANGE:
                    if(inputInt >= thisCondition.compareInts.x &&
                        inputInt <= thisCondition.compareInts.y)
                    {
                        return null;
                    }
                    break;
                case CONDITON_PARAMETER.VALUE_WITHIN_RANGE:
                    if(inputInt < thisCondition.compareInts.x ||
                        inputInt > thisCondition.compareInts.y)
                    {
                        return null;
                    }
                    break;
                case CONDITON_PARAMETER.CONTAINS_SPECIES_GROUP:
                    if(thisCondition.compareSpeciesGroups == null)
                    {
                        return null;
                    }
                    if(inputSpeciesGroups == null)
                    {
                        return null;
                    }

                    bool matchFound = false;

                    for(int sg1 = 0; sg1 < thisCondition.compareSpeciesGroups.Count; sg1++)
                    {
                        for(int sg2 = 0; sg2 < inputSpeciesGroups.Count; sg2++)
                        {
                            if(thisCondition.compareSpeciesGroups.Contains(inputSpeciesGroups[sg2]))
                            {
                                matchFound = true;
                                break;
                            }
                        }
                        if(matchFound) { break; }
                    }

                    if(!matchFound)
                    {
                        return null;
                    }
                    break;
                case CONDITON_PARAMETER.DOES_NOT_CONTAIN_SPECIES_GROUP:
                    if(thisCondition.compareSpeciesGroups == null)
                    {
                        return null;
                    }
                    if(inputSpeciesGroups == null)
                    {
                        return null;
                    }

                    for(int sg1 = 0; sg1 < thisCondition.compareSpeciesGroups.Count; sg1++)
                    {
                        for(int sg2 = 0; sg2 < inputSpeciesGroups.Count; sg2++)
                        {
                            if(thisCondition.compareSpeciesGroups.Contains(inputSpeciesGroups[sg2]))
                            {
                                return null;
                            }
                        }
                    }

                    break;
                case CONDITON_PARAMETER.IS_ALIVE:
                    if(!inputAlive)
                    {
                        return null;
                    }
                    break;
                case CONDITON_PARAMETER.IS_DEAD:
                    if(inputAlive)
                    {
                        return null;
                    }
                    break;                
                case CONDITON_PARAMETER.MATCHES_STATE:
                    if(!inputAlive) { return null; }
                    if(!thisCondition.compareStates.Contains(inputState))
                    {
                        return null;
                    }
                    break;
                case CONDITON_PARAMETER.DOES_NOT_MATCH_STATE:
                    if(thisCondition.compareStates.Contains(inputState))
                    {
                        return null;
                    }
                    break;
                case CONDITON_PARAMETER.IS_WALL_ADJACENT:
                    if(!inputWallAdjacent)
                    {
                        return null;
                    }
                    break;
                case CONDITON_PARAMETER.IS_DYING:
                    if(!inputIsDying)
                    {
                        return null;
                    }
                    break;
                default:
                    Debug.LogError("CONDITION_TYPE of current rule has not yet been set up in the ArbiterScript.");
                    return null;
            }
        }

        return rule.result;
    }
}
