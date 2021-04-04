using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rule
{
    public Rule(string ruleName, Condition[] conditions, Result[] results, bool wallsAreAlive, RULE_CLASSIFICATION classification)
    {
        this.ruleName = ruleName;
        this.conditions = conditions;
        this.results = results;
        this.wallsAreAlive = wallsAreAlive;
        this.classification = classification;
    }

    public string ruleName;
    public Condition[] conditions;
    public Result[] results;
    public bool wallsAreAlive;
    public RULE_CLASSIFICATION classification;
}
