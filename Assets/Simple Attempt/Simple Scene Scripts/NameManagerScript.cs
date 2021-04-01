using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameManagerScript : MonoBehaviour
{
    [SerializeField] GameManagerScript gameManager;

    public void NameSpecies(Species species, string newName)
    {
        SaveData currentSaveData = gameManager.GetCurrentSaveData();

        int foundIndex = -1;

        string defaultName = species.defaultName;

        string[] nameList = currentSaveData.defaultNames;

        int currentArraySize = nameList.Length;

        for(int i = 0; i < currentArraySize; i++)
        {
            string thisName = nameList[i];

            if(defaultName == thisName)
            {
                foundIndex = i;
                break;
            }
        }

        if( foundIndex < 0)
        {
            SaveData newSaveData = new SaveData(new string[currentArraySize + 1], new string[currentArraySize + 1]);

            for(int i = 0; i < currentArraySize; i++)
            {
                newSaveData.defaultNames[i] = currentSaveData.defaultNames[i];
                newSaveData.customNames[i] = currentSaveData.customNames[i];
            }

            newSaveData.defaultNames[currentArraySize] = defaultName;
            newSaveData.customNames[currentArraySize] = newName;

            gameManager.SaveGame(newSaveData);
        }
        else
        {
            currentSaveData.customNames[foundIndex] = newName;
            gameManager.SaveGame();
        }
    }

    public string GetSpeciesName(Species species)
    {
        SaveData currentSaveData = gameManager.GetCurrentSaveData();

        string defaultName = species.defaultName;
        string[] defaultNames = currentSaveData.defaultNames;

        for(int i = 0; i < currentSaveData.defaultNames.Length; i++)
        {
            if(defaultNames[i] == defaultName)
            {
                return currentSaveData.customNames[i];
            }
        }

        return "Unknown";
    }
}
