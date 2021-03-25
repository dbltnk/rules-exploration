using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpeciesAttributes
{
    public SpeciesAttributes(string speciesName, SPECIES speciesEnum, Rule lifeRule, Rule propigationRule, Color aliveColor)
    {
        this.speciesName = speciesName;
        this.speciesEnum = speciesEnum;
        this.lifeRule = lifeRule;
        this.propigationRule = propigationRule;
        this.aliveColor = aliveColor;
    }

    public string speciesName;
    public SPECIES speciesEnum;
    public Rule lifeRule;
    public Rule propigationRule;
    public Color aliveColor;
}

public enum SPECIES
{
    NONE,
    BLOB,
    FLOPPER,
    GOBLIN,
    ROCK,
}

public static class SpeciesReference
{
    /// <summary>
    /// If you want, you can load these in as a base in each level and then modify that reference. This way the species can still be randomized to do whatever you want.
    /// </summary>
    public static SpeciesAttributes[] defaultSpeciesAttributes = new SpeciesAttributes[]
    {
        new SpeciesAttributes("Null", SPECIES.NONE, null, null, Color.black),
        new SpeciesAttributes("Blob", SPECIES.BLOB,
            new Rule(//Life rule
                new Condition[] { new Condition(SOURCE.LIVING_NEIGHBOR_MATCHING_SPECIES_COUNT, CONDITON.VALUE_OUTSIDE_RANGE, new Vector2Int(2, 3), new List<SPECIES> { SPECIES.BLOB, SPECIES.FLOPPER, SPECIES.GOBLIN }) },
                new Result[] { new Result(LIFE_EFFECT.KILL) } ),
            new Rule(//Propigation rule
                new Condition[] { new Condition(SOURCE.LIVING_NEIGHBOR_MATCHING_SPECIES_COUNT, CONDITON.VALUE_WITHIN_RANGE, new Vector2Int(3, 3), SPECIES.BLOB) },
                new Result[] { new Result(LIFE_EFFECT.PROPIGATE, SPECIES.BLOB, STATE.NORMAL) } ),
            new Color(0.75f, 0, 0.72f, 1)),
        new SpeciesAttributes("Flopper", SPECIES.FLOPPER,
            new Rule(//Life rule
                new Condition[] { new Condition(SOURCE.LIVING_NEIGHBOR_MATCHING_SPECIES_COUNT, CONDITON.VALUE_OUTSIDE_RANGE, new Vector2Int(1, 2), new List<SPECIES>{ SPECIES.BLOB, SPECIES.FLOPPER }) },
                new Result[] { new Result(LIFE_EFFECT.KILL) } ),
            new Rule(//Propigation rule
                new Condition[] { new Condition(SOURCE.LIVING_NEIGHBOR_MATCHING_SPECIES_COUNT, CONDITON.VALUE_WITHIN_RANGE, new Vector2Int(2, 2), SPECIES.BLOB) },
                new Result[] { new Result(LIFE_EFFECT.PROPIGATE, SPECIES.FLOPPER, STATE.HAPPY) } ),
            new Color(0.81f, 0.68f, 0.16f, 1)),
        new SpeciesAttributes("Goblin", SPECIES.GOBLIN,
            new Rule(//Life rule
                new Condition[] { new Condition(SOURCE.LIVING_NEIGHBOR_MATCHING_SPECIES_COUNT, CONDITON.VALUE_OUTSIDE_RANGE, new Vector2Int(3, 4), new List<SPECIES>{ SPECIES.GOBLIN, SPECIES.FLOPPER, SPECIES.ROCK }) },
                new Result[] { new Result(LIFE_EFFECT.KILL) } ),
            new Rule(//Propigation rule
                new Condition[] { new Condition(SOURCE.LIVING_NEIGHBOR_MATCHING_SPECIES_COUNT, CONDITON.VALUE_WITHIN_RANGE, new Vector2Int(1, 1), new List<SPECIES>{ SPECIES.GOBLIN, SPECIES.FLOPPER, SPECIES.ROCK } ),
                                    new Condition(SOURCE.RANDOM_D6, CONDITON.VALUE_WITHIN_RANGE, new Vector2Int(6, 6)) },//Goblins additionally only a chance of spawning.
                new Result[] { new Result(LIFE_EFFECT.PROPIGATE, SPECIES.GOBLIN, STATE.SICKLY) } ),
            new Color(0.16f, 0.65f, 0.05f, 1)),
        new SpeciesAttributes("Rock", SPECIES.ROCK,
            null,//Rocks don't die of being lonely.
            null,//Propigation rule (rocks don't)
            new Color(0.75f, 0.75f, 0.75f, 1)),
    };
}
