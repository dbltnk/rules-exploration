using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rule
{
    public Rule(string ruleName, Condition[] conditions, Result[] results, NEIGHBOR_STYLE neighborStyle, bool wallsAreAlive, RULE_CLASSIFICATION classification)
    {
        this.ruleName = ruleName;
        this.conditions = conditions;
        this.results = results;
        this.neighborStyle = neighborStyle;
        this.wallsAreAlive = wallsAreAlive;
        this.classification = classification;
    }

    public Rule(RuleObject ruleObject)
    {
        ruleName = ruleObject.name;
        conditions = ruleObject.conditions;
        results = ruleObject.results;
        neighborStyle = ruleObject.neighborStyle;
        wallsAreAlive = ruleObject.wallsAreAlive;
        classification = ruleObject.classification;
    }

    public string ruleName;
    public Condition[] conditions;
    public Result[] results;
    public NEIGHBOR_STYLE neighborStyle;
    public bool wallsAreAlive;
    public RULE_CLASSIFICATION classification;
}
