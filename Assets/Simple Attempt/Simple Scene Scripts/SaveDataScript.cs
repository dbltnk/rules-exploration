using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class SaveData
{
    public SaveData(string[] customNames)
    {
        this.customNames = customNames;
    }

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

    public static void SaveGame(SaveData saveData)
    {
        BinaryFormatter bianaryFormatter = new BinaryFormatter();
        FileStream filestream = new FileStream(filePath, FileMode.Create);

        bianaryFormatter.Serialize(filestream, saveData);
        filestream.Close();
    }
}
