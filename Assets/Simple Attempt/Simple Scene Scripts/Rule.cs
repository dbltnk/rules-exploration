using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Rule
{
    public Rule(int ruleIndex, Condition[] conditions, Result result, NEIGHBOR_STYLE neighborStyle, bool wallsAreAlive, RULE_CLASSIFICATION classification)
    {
        this.ruleIndex = ruleIndex;
        this.conditions = conditions;
        this.result = result;
        this.neighborStyle = neighborStyle;
        this.wallsAreAlive = wallsAreAlive;
        this.classification = classification;
    }

    public Rule(RuleObject ruleObject, int ruleIndex)
    {
        this.ruleIndex = ruleIndex;

        int conditionCount = Random.Range(Mathf.Max(1, ruleObject.possibleConditionAmounts.x), Mathf.Max(2, ruleObject.possibleConditionAmounts.y + 1));//Adding one to the right because the max is exclusive.
        List<Condition> unusedConditions = new List<Condition>();
        List<Condition> finalConditionList = new List<Condition>();
        unusedConditions.AddRange(ruleObject.possibleConditions);
        for(int i = 0; i < conditionCount; i++)
        {
            int chosenConditionInt = Random.Range(0, unusedConditions.Count);
            Condition chosenCondition = unusedConditions[chosenConditionInt];
            if(ruleObject.randomizeCompareInts)
            {
                int minimum = Random.Range(ruleObject.compareIntsRandomMinRange[0], ruleObject.compareIntsRandomMinRange[1] + 1);
                int maximum = Random.Range(ruleObject.compareIntsRandomMaxRange[0], ruleObject.compareIntsRandomMaxRange[1] + 1);
                chosenCondition.compareInts = new Vector2Int(minimum, maximum);
            }
            finalConditionList.Add(chosenCondition);
            unusedConditions.RemoveAt(chosenConditionInt);
        }
        conditions = finalConditionList.ToArray();

        if(ruleObject.possibleResults != null)
        {
            if(ruleObject.possibleResults.Length > 0)
            {
                result = ruleObject.possibleResults[Random.Range(0, ruleObject.possibleResults.Length)];
            }            
        }        

        neighborStyle = ruleObject.neighborStyle;
        if(neighborStyle == NEIGHBOR_STYLE.RANDOM) { neighborStyle = (NEIGHBOR_STYLE)Random.Range(0, (int)NEIGHBOR_STYLE.RANDOM); }

        switch(ruleObject.wallsAreAlive)
        {
            case RO_WALLS_ALIVE.FALSE:
                wallsAreAlive = false;
                break;
            case RO_WALLS_ALIVE.TRUE:
                wallsAreAlive = true;
                break;
            default:// case RO_WALLS_ALIVE.RANDOM:
                wallsAreAlive = Random.Range(0, 2) == 1;
                break;
        }

        classification = ruleObject.classification;
    }

    public int ruleIndex;
    public Condition[] conditions;
    public Result result;
    public NEIGHBOR_STYLE neighborStyle;
    public bool wallsAreAlive;
    public RULE_CLASSIFICATION classification;
}
