using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulesBank : MonoBehaviour
{
    [SerializeField] GameManagerScript gameManager;

    [SerializeField] RuleObject[] ruleObjectArray;

    Rule[] rulesBank;//This holds the actual rules that are used in the game.

    //These hold rules objects from which rules area created.
    List<RuleObject> birthRuleObjects;
    List<RuleObject> deathRuleObjects;

    public void AssignRuleObjectArray(RuleObject[] ruleObjectArray)
    {
        this.ruleObjectArray = ruleObjectArray;
    }

    void Awake()
    {
        InitializeBirthAndDeathRuleBases();
    }

    void InitializeBirthAndDeathRuleBases()
    {
        birthRuleObjects = new List<RuleObject>();
        deathRuleObjects = new List<RuleObject>();

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
            }
        }
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

            rulesBank[i] = new Rule(ruleObjectArray[i], i);
        }        
    }

    Rule DeserializeRule(int ruleIndex, int ruleClassification, int neighborStyle, bool wallsAreAlive, int[] conditionSource, int[] conditionParameters, int[][] compareInts, int[][] compareSpeciesGroups,
        int[][] compareStates, int lifeEffect, int newState)
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

        return new Rule(ruleIndex, conditions, result, (NEIGHBOR_STYLE)neighborStyle, wallsAreAlive, (RULE_CLASSIFICATION)ruleClassification);
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
                saveData.ruleResultLifeEffect[i], saveData.ruleResultNewState[i]);
        }
    }

    public Rule[] GetRulesBank() { return rulesBank; }

    public Rule GetRule(int index)
    {
        return rulesBank[index];
    }

    public Rule GetRandomBirthRule()
    {
        return new Rule(birthRuleObjects[Random.Range(0, birthRuleObjects.Count)], rulesBank.Length);
    }

    public Rule GetRandomDeathRule()
    {
        return new Rule(deathRuleObjects[Random.Range(0, deathRuleObjects.Count)], rulesBank.Length);
    }

    public Rule CreateNewRule(RuleObject ruleObjectBase)
    {   
        int ruleBankLength = rulesBank.Length;

        Rule newRule = new Rule(ruleObjectBase, ruleBankLength);

        Rule[] newRuleBank = new Rule[rulesBank.Length + 1];

        for(int i = 0; i < ruleBankLength; i++)
        {
            newRuleBank[i] = rulesBank[i];
        }

        newRuleBank[newRuleBank.Length - 1] = newRule;

        rulesBank = newRuleBank;

        gameManager.SaveGame();

        return newRule;
    }
}
