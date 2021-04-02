using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class SaveData
{
    public SaveData(SerializedSpecies[] serializedSpeciesArray, string[] currentCustomNames)
    {
        int speciesCount = serializedSpeciesArray.Length;

        defaultNames = new string[speciesCount];
        customNames = new string[speciesCount];
        speciesGroups = new int[speciesCount][];
        speciesColors = new float[speciesCount][];
        startingPopulations = new int[speciesCount];
        birthRuleIndex = new int[speciesCount];
        deathRuleIndex = new int[speciesCount];

        for(int i = 0; i < speciesCount; i++)
        {
            defaultNames[i] = serializedSpeciesArray[i].defaultName;
            customNames[i] = currentCustomNames[i];
            speciesGroups[i] = serializedSpeciesArray[i].speciesGroups;
            speciesColors[i] = serializedSpeciesArray[i].color;
            startingPopulations[i] = serializedSpeciesArray[i].startingPopulation;
            birthRuleIndex[i] = serializedSpeciesArray[i].birthRuleIndex;
            deathRuleIndex[i] = serializedSpeciesArray[i].deathRuleIndex;
        }
    }

    public string[] defaultNames;//Default names of species that have saved custom names
    public string[] customNames;
    public int[][] speciesGroups;
    public float[][] speciesColors;
    public int[] startingPopulations;
    public int[] birthRuleIndex;
    public int[] deathRuleIndex;
}

public class SerializedSpecies
{
    public SerializedSpecies(string defaultNameInc, List<SPECIES_GROUP> speciesGroupsInc, Color colorInc, SPECIES_STARTING_POPULATION startingPopulationInc, int birthRuleInc, int deathRuleInc)
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
    }

    public string defaultName;
    public int[] speciesGroups;
    public float[] color;
    public int startingPopulation;
    public int birthRuleIndex;
    public int deathRuleIndex;
}

public static class SaveDataScript
{
    static string filePath = Application.persistentDataPath + "/connie.wei";

    public static SaveData LoadSaveData()
    {
        if(File.Exists(filePath))
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
