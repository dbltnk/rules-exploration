using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RulesBank : MonoBehaviour
{
    [SerializeField] GameManagerScript gameManager;

    [SerializeField] RuleObject[] ruleObjectArray;

    Rule[] rulesBank;//This holds the actual rules that are used in the game.

    //These hold rules objects from which rules area created.
    List<RuleObject> birthRuleObjects;
    List<RuleObject> deathRuleObjects;
    List<RuleObject> otherRuleObjects;

    void Awake()
    {
        ruleObjectArray = Resources.LoadAll("Rules", typeof(RuleObject)).Cast<RuleObject>().ToArray();
        InitializeBirthAndDeathRuleBases();
    }

    void InitializeBirthAndDeathRuleBases()
    {
        birthRuleObjects = new List<RuleObject>();
        deathRuleObjects = new List<RuleObject>();
        otherRuleObjects = new List<RuleObject>();

        for(int i = 0; i < ruleObjectArray.Length; i++)
        {
            RuleObject thisRuleObject = ruleObjectArray[i];
            switch(thisRuleObject.classification)
            {
                case RULE_CLASSIFICATION.BIRTH:
                    birthRuleObjects.Add(thisRuleObject);
                    break;
                case RULE_CLASSIFICATION.DEATH:
                    deathRuleObjects.Add(thisRuleObject);
                    break;
                default:
                    otherRuleObjects.Add(thisRuleObject);
                    break;
            }
        }
    }

    /// <summary>
    /// When creating new save data, this builds the foundation of the rules bank.
    /// </summary>
    public void InitializeNewRulesBank()
    {
        rulesBank = new Rule[0];    
    }

    Rule DeserializeRule(int ruleIndex, int ruleClassification, int neighborStyle, bool wallsAreAlive, int[] conditionSource, int[] conditionParameters, int[][] compareInts, int[][] compareSpeciesGroups,
        int[][] compareStates, int lifeEffect, int newState, bool nullRule)
    {
        int conditionAmount = conditionSource.Length;

        Condition[] conditions = new Condition[conditionAmount];   

        for(int i = 0; i < conditionAmount; i++)
        {
            List<SPECIES_GROUP> compareSpeciesGroupList = new List<SPECIES_GROUP>();

            int[] compareSpeciesIntArray = compareSpeciesGroups[i];

            for(int c = 0; c < compareSpeciesIntArray.Length; c++)
            {
                compareSpeciesGroupList.Add((SPECIES_GROUP)compareSpeciesIntArray[c]);
            }

            conditions[i] = new Condition((SOURCE)conditionSource[i], (CONDITON_PARAMETER)conditionParameters[i], new Vector2Int(compareInts[i][0], compareInts[i][1]), compareSpeciesGroupList);
        }

        Result result = new Result((LIFE_EFFECT)lifeEffect, (STATE)newState);

        return new Rule(ruleIndex, conditions, result, (NEIGHBOR_STYLE)neighborStyle, wallsAreAlive, (RULE_CLASSIFICATION)ruleClassification, nullRule);
    }

    public void LoadSavedRuleBank(SaveData saveData)
    {
        if(saveData.ruleIndexes == null)
        {
            gameManager.CreateNewGameSave();
            return;
        }        

        int ruleAmount = saveData.ruleIndexes.Length;

        rulesBank = new Rule[ruleAmount];

        for(int i = 0; i < ruleAmount; i++)
        {
            rulesBank[i] = DeserializeRule(saveData.ruleIndexes[i], saveData.ruleClassifications[i], saveData.ruleNeighborStyle[i], saveData.ruleWallsAreAlive[i], saveData.ruleConditionSource[i],
                saveData.ruleConditionParameters[i], saveData.ruleCompareInts[i], saveData.ruleCompareSpeciesGroups[i], saveData.ruleCompareStates[i],
                saveData.ruleResultLifeEffect[i], saveData.ruleResultNewState[i], saveData.ruleNullRule[i]);
        }
    }

    public Rule[] GetRulesBank() { return rulesBank; }

    public Rule GetRule(int index)
    {
        return rulesBank[index];
    }

    public Rule GetRandomBirthRule(List<SPECIES_GROUP> speciesGroups)
    {
        Rule newRule = new Rule(birthRuleObjects[Random.Range(0, birthRuleObjects.Count)], rulesBank.Length, speciesGroups);
        RegisterNewRule(newRule);
        return newRule;
    }

    public Rule GetRandomDeathRule(List<SPECIES_GROUP> speciesGroups)
    {
        Rule newRule = new Rule(deathRuleObjects[Random.Range(0, deathRuleObjects.Count)], rulesBank.Length, speciesGroups);
        RegisterNewRule(newRule);
        return newRule;
    }

    public Rule[] GetRandomOtherRules(List<SPECIES_GROUP> speciesGroups, int amount)
    {
        if(amount < 1) { return new Rule[0]; }
        if(otherRuleObjects.Count < 1) { return new Rule[0]; }

        Rule[] newRuleArray = new Rule[amount];

        for(int i = 0; i < amount; i++)
        {
            Rule newRule = new Rule(otherRuleObjects[Random.Range(0, otherRuleObjects.Count)], rulesBank.Length, speciesGroups);
            RegisterNewRule(newRule);
            newRuleArray[i] = newRule;
        }        

        return newRuleArray;
    }

    public Rule GetRuleFromRuleObjectAtRuntime(RuleObject ruleObject, List<SPECIES_GROUP> speciesGroups)
    {
        Rule newRule = new Rule(ruleObject, rulesBank.Length, speciesGroups);

        RegisterNewRule(newRule);

        return newRule;
    }

    void RegisterNewRule(Rule newRule)
    {
        int ruleIndex = newRule.ruleIndex;

        int ruleBankLength = rulesBank.Length;

        Rule[] newRuleBank = new Rule[ruleBankLength + 1];

        for(int i = 0; i < ruleBankLength; i++)
        {
            newRuleBank[i] = rulesBank[i];
        }

        newRuleBank[newRuleBank.Length - 1] = newRule;

        rulesBank = newRuleBank;

        gameManager.SaveGame();
    }
}
