using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class SaveData
{
    public SaveData(SerializedSpecies[] serializedSpeciesArray, string[] currentCustomNames, SerializedRule[] serializedRulesArray)
    {
        int speciesCount = serializedSpeciesArray.Length;

        defaultNames = new string[speciesCount];
        customNames = new string[speciesCount];
        speciesGroups = new int[speciesCount][];
        speciesColors = new float[speciesCount][];
        startingPopulations = new int[speciesCount];
        birthRuleIndex = new int[speciesCount];
        deathRuleIndex = new int[speciesCount];
        otherRulesIndexes = new int[speciesCount][];

        for (int i = 0; i < speciesCount; i++)
        {
            defaultNames[i] = serializedSpeciesArray[i].defaultName;
            customNames[i] = currentCustomNames[i];
            speciesGroups[i] = serializedSpeciesArray[i].speciesGroups;
            speciesColors[i] = serializedSpeciesArray[i].color;
            startingPopulations[i] = serializedSpeciesArray[i].startingPopulation;
            birthRuleIndex[i] = serializedSpeciesArray[i].birthRuleIndex;
            deathRuleIndex[i] = serializedSpeciesArray[i].deathRuleIndex;
            otherRulesIndexes[i] = serializedSpeciesArray[i].otherRulesIndexes;
        }

        int ruleCount = serializedRulesArray.Length;

        ruleIndexes = new int[ruleCount];
        ruleClassifications = new int[ruleCount];
        ruleWallsAreAlive = new bool[ruleCount];
        ruleConditionSource = new int[ruleCount][];
        ruleConditionParameters = new int[ruleCount][];
        ruleCompareInts = new int[ruleCount][][];
        ruleCompareSpeciesGroups = new int[ruleCount][][];
        ruleCompareStates = new int[ruleCount][][];
        ruleResultLifeEffect = new int[ruleCount];
        ruleResultNewState = new int[ruleCount];
        ruleNeighborStyle = new int[ruleCount];
        ruleNullRule = new bool[ruleCount];

        for(int i = 0; i < ruleCount; i++)
        {
            SerializedRule thisRule = serializedRulesArray[i];
            int conditionsCount = thisRule.conditions.Length;

            ruleIndexes[i] = thisRule.ruleIndex;
            ruleClassifications[i] = thisRule.classification;
            ruleNeighborStyle[i] = thisRule.neighborStyle;
            ruleWallsAreAlive[i] = thisRule.wallsAreAlive;
            ruleConditionSource[i] = new int[conditionsCount];
            for(int c = 0; c < conditionsCount; c++)
            {
                ruleConditionSource[i][c] = thisRule.conditions[c].source;
            }
            ruleConditionParameters[i] = new int[conditionsCount];
            for(int c = 0; c < conditionsCount; c++)
            {
                ruleConditionParameters[i][c] = thisRule.conditions[c].conditionParameter;
            }
            ruleCompareInts[i] = new int[conditionsCount][];
            for(int c = 0; c < conditionsCount; c++)
            {
                ruleCompareInts[i][c] = thisRule.conditions[c].compareInts;
            }
            ruleCompareSpeciesGroups[i] = new int[conditionsCount][];
            for(int c = 0; c < conditionsCount; c++)
            {
                ruleCompareSpeciesGroups[i][c] = thisRule.conditions[c].compareSpeciesGroups;
            }
            ruleCompareStates[i] = new int[conditionsCount][];
            for(int c = 0; c < conditionsCount; c++)
            {
                ruleCompareStates[i][c] = thisRule.conditions[c].compareStates;
            }
            ruleResultLifeEffect[i] = thisRule.result.lifeEffect;
            ruleResultNewState[i] = thisRule.result.newState;
            ruleNullRule[i] = thisRule.nullRule;
        }
    }

    //Species Data
    public string[] defaultNames;//Default names of species that have saved custom names
    public string[] customNames;
    public int[][] speciesGroups;
    public float[][] speciesColors;
    public int[] startingPopulations;
    public int[] birthRuleIndex;
    public int[] deathRuleIndex;
    public int[][] otherRulesIndexes;

    //Rules Data
    public int[] ruleIndexes;
    public int[] ruleClassifications;
    public int[] ruleNeighborStyle;
    public bool[] ruleWallsAreAlive;
    public int[][] ruleConditionSource;
    public int[][] ruleConditionParameters;
    public int[][][] ruleCompareInts;
    public int[][][] ruleCompareSpeciesGroups;
    public int[][][] ruleCompareStates;
    public int[] ruleResultLifeEffect;
    public int[] ruleResultNewState;
    public bool[] ruleNullRule;
}

public enum RULE_CLASSIFICATION
{
    OTHER,
    BIRTH,
    DEATH,
}

public class SerializedRule
{
    public SerializedRule(Rule rule)
    {
        ruleIndex = rule.ruleIndex;
        conditions = new SerializedCondition[rule.conditions.Length];
        for(int i = 0; i < conditions.Length; i++)
        {
            conditions[i] = new SerializedCondition(rule.conditions[i]);
        }
        result = new SerializedResult(rule.result);
        classification = (int)rule.classification;
        neighborStyle = (int)rule.neighborStyle;
        wallsAreAlive = rule.wallsAreAlive;
        nullRule = rule.nullRule;
    }

    public int ruleIndex;
    public SerializedCondition[] conditions;
    public SerializedResult result;
    public int classification;
    public int neighborStyle;
    public bool wallsAreAlive;
    public bool nullRule;
}

public class SerializedCondition
{
    public SerializedCondition(Condition condition)
    {
        source = (int)condition.source;
        this.conditionParameter = (int)condition.conditionParameter;
        compareInts = new int[] { condition.compareInts.x, condition.compareInts.y };
        compareSpeciesGroups = new int[condition.compareSpeciesGroups.Count];
        for(int i = 0; i < compareSpeciesGroups.Length; i++)
        {
            compareSpeciesGroups[i] = (int)condition.compareSpeciesGroups[i];
        }
        compareStates = new int[condition.compareStates.Count];
        for(int i = 0; i < compareStates.Length; i++)
        {
            compareStates[i] = (int)condition.compareStates[i];
        }
    }

    public int source;
    public int conditionParameter;
    public int[] compareInts;
    public int[] compareSpeciesGroups;
    public int[] compareStates;
}

public class SerializedResult
{
    public SerializedResult(Result result)
    {
        if(result == null)
        {
            lifeEffect = (int)LIFE_EFFECT.NONE;
            newState = (int)STATE.NONE;
        }
        else
        {
            lifeEffect = (int)result.lifeEffect;
            newState = (int)result.newState;
        }        
    }

    public int lifeEffect;
    public int newState;
}

public class SerializedSpecies
{
    public SerializedSpecies(string defaultNameInc, List<SPECIES_GROUP> speciesGroupsInc, Color colorInc, SPECIES_STARTING_POPULATION startingPopulationInc, int birthRuleInc, int deathRuleInc, Rule[] otherRulesIndexesInc)
    {
        defaultName = defaultNameInc;
        speciesGroups = new int[speciesGroupsInc.Count];
        for(int i = 0; i < speciesGroupsInc.Count; i++)
        {
            speciesGroups[i] = (int)speciesGroupsInc[i];
        }
        color = new float[]
        {
            colorInc.r, colorInc.g, colorInc.b, colorInc.a
        };
        startingPopulation = (int)startingPopulationInc;
        birthRuleIndex = birthRuleInc;
        deathRuleIndex = deathRuleInc;
        if(otherRulesIndexesInc == null)
        {
            otherRulesIndexes = new int[0];
        }
        else
        {
            otherRulesIndexes = new int[otherRulesIndexesInc.Length];

            if(otherRulesIndexesInc.Length > 0)
            {
                for(int i = 0; i < otherRulesIndexes.Length; i++)
                {
                    otherRulesIndexes[i] = otherRulesIndexesInc[i].ruleIndex;
                }
            }
        }              
    }

    public string defaultName;
    public int[] speciesGroups;
    public float[] color;
    public int startingPopulation;
    public int birthRuleIndex;
    public int deathRuleIndex;
    public int[] otherRulesIndexes;
}

public static class SaveDataScript
{
    static string filePath = Application.persistentDataPath + "/connie.wei";

    public static SaveData LoadSaveData()
    {
        if (File.Exists(filePath))
        {
            BinaryFormatter bianaryFormatter = new BinaryFormatter();
            FileStream filestream = new FileStream(filePath, FileMode.Open);

            SaveData saveData = (SaveData)bianaryFormatter.Deserialize(filestream);

            filestream.Close();

            return saveData;
        }

        return null;
    }

    public static SaveData SaveGame(SaveData saveData)
    {
        BinaryFormatter bianaryFormatter = new BinaryFormatter();
        FileStream filestream = new FileStream(filePath, FileMode.Create);

        bianaryFormatter.Serialize(filestream, saveData);
        filestream.Close();

        return saveData;
    }

    public static void DeleteSaveData()
    {
        File.Delete(filePath);
    }
}
