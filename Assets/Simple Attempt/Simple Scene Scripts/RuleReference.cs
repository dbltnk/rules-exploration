using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RULE_REFERENCE
{
    STANDARD_DEATH,
    STANDARD_PROPIGATION,
    SICK_CELLS_CAN_DIE,
    FLOPPERS_CAN_BECOME_BLOBS,    
    BLOBS_EAT_ROCKS,
    FLOPPERS_HATE_GOBLINS,
    DISEASE_CAN_SPREAD,
    DISEASE_CAN_CLEAR_UP,
    FINAL_RULE_LEAVE_EMPTY//Scramble currently depends on this as being the final rule enum.
}

public enum DEATH_RULES
{
    ROCKS_ARE_IMMORTAL,
    FINAL_RULE_LEAVE_EMPTY//Scramble currently depends on this as being the final rule enum.
}

public static class RuleReference
{
    public static Rule[] premadeRuleArray = new Rule[]
    {
        new Rule( //STANDAR_DEATH: Cells dies unless they have 2-3 neighbors.
        new Condition[]
        {
            new Condition(SOURCE.LIVING_NEIGHBOR_COUNT, CONDITON.VALUE_OUTSIDE_RANGE, new Vector2Int(2, 3)),
        },
        new Result[]
        {
            new Result(LIFE_EFFECT.KILL)
        }
        ),

        new Rule( //STANDARD_PROPIGATION: Cells born if they have exactly three neighbors.
        new Condition[]
        {
            new Condition(SOURCE.LIVING_NEIGHBOR_COUNT, CONDITON.VALUE_WITHIN_RANGE, new Vector2Int(3, 3), STATE.SICKLY),
        },
        new Result[]
        {
            new Result(LIFE_EFFECT.PROPIGATE)
        }
        ),

        new Rule( //SICK_CELLS_CAN_DIE
        new Condition[]
        {
            new Condition(SOURCE.TARGET, CONDITON.VALUE_MATCHES_STATE, STATE.SICKLY),
            new Condition(SOURCE.RANDOM_D6, CONDITON.VALUE_WITHIN_RANGE, new Vector2Int(1, 1)),
        },
        new Result[]
        {
            new Result(LIFE_EFFECT.KILL)
        }
        ),

        new Rule( //FLOPPERS_CAN_BECOME_BLOBS if they are normal and roll high.
        new Condition[]
        {
            new Condition(SOURCE.TARGET, CONDITON.VALUE_MATCHES_STATE, STATE.NORMAL),
            new Condition(SOURCE.RANDOM_D6, CONDITON.VALUE_WITHIN_RANGE, new Vector2Int(5, 6)),
            new Condition(SOURCE.TARGET, CONDITON.VALUE_MATCHES_SPECIES, SPECIES.FLOPPER),
        },
        new Result[]
        {
            new Result(SPECIES.BLOB)
        }
        ),        

        new Rule( //BLOBS_EAT_ROCKS
        new Condition[]
        {
            new Condition(SOURCE.TARGET, CONDITON.VALUE_MATCHES_SPECIES, SPECIES.ROCK),
            new Condition(SOURCE.NEIGHBOR_DE, CONDITON.VALUE_MATCHES_SPECIES, SPECIES.BLOB),
            new Condition(SOURCE.NEIGHBOR_DW, CONDITON.VALUE_MATCHES_SPECIES, SPECIES.BLOB),
            new Condition(SOURCE.NEIGHBOR_DN, CONDITON.VALUE_MATCHES_SPECIES, SPECIES.BLOB),
            new Condition(SOURCE.NEIGHBOR_DS, CONDITON.VALUE_MATCHES_SPECIES, SPECIES.BLOB),
        },
        new Result[]
        {
            new Result(SPECIES.BLOB)
        }
        ),

        new Rule( //FLOPPERS_HATE_GOBLINS
        new Condition[]
        {
            new Condition(SOURCE.TARGET, CONDITON.VALUE_MATCHES_SPECIES, SPECIES.FLOPPER),
            new Condition(SOURCE.TARGET, CONDITON.VALUE_MATCHES_STATE, STATE.HAPPY),
            new Condition(SOURCE.LIVING_NEIGHBOR_MATCHING_SPECIES_COUNT, CONDITON.VALUE_OUTSIDE_RANGE, new Vector2Int(1, 1), SPECIES.GOBLIN),
            new Condition(SOURCE.RANDOM_D6, CONDITON.VALUE_WITHIN_RANGE, new Vector2Int(1, 3)),
        },
        new Result[]
        {
            new Result(STATE.NORMAL)
        }
        ),

        new Rule( //DISEASE_CAN_SPREAD
        new Condition[]
        {
            new Condition(SOURCE.LIVING_NEIGHBORS_MATCHING_STATE_COUNT, CONDITON.VALUE_MATCHES_SPECIES, new Vector2Int(1, 6), STATE.SICKLY),
            new Condition(SOURCE.TARGET, CONDITON.VALUE_DOES_NOT_MATCH_STATE, STATE.SICKLY),
            new Condition(SOURCE.RANDOM_D6, CONDITON.VALUE_WITHIN_RANGE, new Vector2Int(1, 2))
        },
        new Result[]
        {
            new Result(STATE.SICKLY)
        }
        ),

        new Rule( //DISEASE_CAN_CLEAR_UP
        new Condition[]
        {
            new Condition(SOURCE.LIVING_NEIGHBORS_MATCHING_STATE_COUNT, CONDITON.VALUE_OUTSIDE_RANGE, new Vector2Int(1, 6), STATE.SICKLY),
            new Condition(SOURCE.TARGET, CONDITON.VALUE_MATCHES_STATE, STATE.SICKLY),
            new Condition(SOURCE.RANDOM_D6, CONDITON.VALUE_WITHIN_RANGE, new Vector2Int(5, 6))
        },
        new Result[]
        {
            new Result(STATE.NORMAL)
        }
        ),
    };

    public static Rule[] premadeDeathRules = new Rule[]
    {
        new Rule( //ROCKS_ARE_IMMORTAL
        new Condition[]
        {
            new Condition(SOURCE.TARGET, CONDITON.CELL_IS_DYING),
            new Condition(SOURCE.TARGET, CONDITON.VALUE_MATCHES_SPECIES, SPECIES.ROCK),
        },
        new Result[]
        {
            new Result(LIFE_EFFECT.PROPIGATE)
        }
        ),
    };
}
