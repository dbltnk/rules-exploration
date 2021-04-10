using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Rule
{
    public Rule(int ruleIndex, Condition[] conditions, Result result, NEIGHBOR_STYLE neighborStyle, bool wallsAreAlive, RULE_CLASSIFICATION classification, bool nullRule)
    {
        this.ruleIndex = ruleIndex;
        this.conditions = conditions;
        this.result = result;
        this.neighborStyle = neighborStyle;
        this.wallsAreAlive = wallsAreAlive;
        this.classification = classification;
    }

    public Rule(RuleObject ruleObject, int ruleIndex, List<SPECIES_GROUP> speciesGroups)
    {
        this.ruleIndex = ruleIndex;

        if(ruleObject == null)
        {
            conditions = new Condition[0];
            result = new Result(LIFE_EFFECT.NONE, STATE.NONE);
            neighborStyle = NEIGHBOR_STYLE.CARDINAL_ONLY;
            wallsAreAlive = false;
            classification = RULE_CLASSIFICATION.OTHER;
            nullRule = true;
            return;
        }

        int conditionCount = Random.Range(Mathf.Max(1, ruleObject.possibleConditionAmounts.x), Mathf.Max(2, ruleObject.possibleConditionAmounts.y + 1));//Adding one to the right because the max is exclusive.
        List<Condition> unusedConditions = new List<Condition>();
        List<Condition> finalConditionList = new List<Condition>();
        unusedConditions.AddRange(ruleObject.possibleConditions);

        conditionCount = Mathf.Min(conditionCount, unusedConditions.Count);

        for(int i = 0; i < conditionCount; i++)
        {
            int chosenConditionInt = Random.Range(0, unusedConditions.Count);
            Condition chosenCondition = unusedConditions[chosenConditionInt];
            if(ruleObject.randomizeCompareInts)
            {
                int minimum = Random.Range(ruleObject.compareIntsRandomMinRange[0], ruleObject.compareIntsRandomMinRange[1] + 1);
                int maximum = Random.Range(ruleObject.compareIntsRandomMaxRange[0], ruleObject.compareIntsRandomMaxRange[1] + 1);
                maximum = Mathf.Max(minimum, maximum);//In case the minimum rolls higher than the maximum, they will just be the same.
                chosenCondition.compareInts = new Vector2Int(minimum, maximum);
            }

            if(speciesGroups != null)
            {
                List<SPECIES_GROUP> unusedSpeciesGroups = new List<SPECIES_GROUP>();

                for(int s = 0; s < (int)SPECIES_GROUP.FINAL_ENTRY_DO_NOT_REPLACE; s++)
                {
                    SPECIES_GROUP thisSpeciesGroup = (SPECIES_GROUP)s;
                    if(ruleObject.matchMySpeciesGroups &&
                        speciesGroups.Contains(thisSpeciesGroup))
                    {
                        chosenCondition.compareSpeciesGroups.Add(thisSpeciesGroup);
                    }
                    else
                    {
                        unusedSpeciesGroups.Add(thisSpeciesGroup);
                    }                    
                }

                if(ruleObject.addRandomSpeciesGroups)
                {
                    for(int rg = 0; rg < Mathf.Min(Random.Range(1, 3), unusedSpeciesGroups.Count); rg++)
                    {
                        AddRandomSpeciesGroup();
                    }

                    void AddRandomSpeciesGroup()
                    {
                        int speciesIndex = Random.Range(0, unusedSpeciesGroups.Count);
                        chosenCondition.compareSpeciesGroups.Add(unusedSpeciesGroups[speciesIndex]);
                        unusedSpeciesGroups.RemoveAt(speciesIndex);
                    }
                }                
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
    public bool nullRule = false;
}
