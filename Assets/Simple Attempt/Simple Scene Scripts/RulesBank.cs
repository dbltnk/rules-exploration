using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulesBank : MonoBehaviour
{
    [SerializeField] GameManagerScript gameManager;

    RuleObject[] ruleObjectArray;

    Rule[] rulesBank;

    List<Rule> birthRules;
    List<Rule> deathRules;

    private void Awake()
    {
        
    }

    /// <summary>
    /// When creating new save data, this builds the foundation of the rules bank.
    /// </summary>
    public void InitializeNewRulesBank()
    {
        int rulesCount = ruleObjectArray.Length;

        rulesBank = new Rule[rulesCount];

        for(int i = 0; i < rulesCount; i++)
        {
            Debug.Log("Converting a rule object to a rule.");

            rulesBank[i] = new Rule(ruleObjectArray[i]);
        }

        for(int i = 0; i < rulesBank.Length; i++)
        {
            Rule thisRule = rulesBank[i];
            switch(thisRule.classification)
            {
                case RULE_CLASSIFICATION.BIRTH:
                    birthRules.Add(thisRule);
                    break;
                case RULE_CLASSIFICATION.DEATH:
                    deathRules.Add(thisRule);
                    break;
            }
        }
    }

    Rule DeserializeRule(string ruleName, int ruleClassification, bool wallsAreAlive, int[] conditionSource, int[] conditionParameters, int[][] compareInts, int[][] compareSpeciesGroups,
        int[][] compareStates, int[] lifeEffects, int[] newStates)
    {
        Condition[] conditions;
        Result[] results;

        return new Rule(ruleName, conditions, results, (NEIGHBOR_STYLE)neighborStyle, wallsAreAlive, (RULE_CLASSIFICATION)ruleClassification);
    }

    public void LoadSavedRuleBank(SaveData saveData)
    {

    }

    public void AssignRuleObjects(RuleObject[] ruleObjectArray)
    {
        this.ruleObjectArray = ruleObjectArray;
    }

    public Rule[] GetRulesBank() { return rulesBank; }

    public Rule GetRule(int index)
    {
        return rulesBank[index];
    }

    public Rule GetRandomBirthRule()
    {
        return birthRules[Random.Range(0, birthRules.Count)];
    }

    public Rule GetRandomDeathRule()
    {
        return deathRules[Random.Range(0, deathRules.Count)];
    }

    public int GetIndexOfRule(Rule rule)
    {
        string ruleName = rule.ruleName;

        for(int i = 0; i < rulesBank.Length; i++)
        {
            if(ruleName == rulesBank[i].ruleName)
            {
                return i;
            }
        }

        Debug.LogError("The given rule was not found within the rules bank.");

        return -6;
    }
}
