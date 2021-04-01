using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class SaveData
{
    public SaveData(string[] defaultNames, string[] customNames)
    {
        this.defaultNames = defaultNames;
        this.customNames = customNames;
    }

    public string[] defaultNames;//Default names of species that have saved custom names
    public string[] customNames;
}

public static class SaveDataScript
{
    static string filePath = Application.persistentDataPath + "/connie.wie";

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
}
