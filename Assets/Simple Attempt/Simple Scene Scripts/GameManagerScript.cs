using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SCENE
{
    LEVEL_SETUP = 14,//Values pulled from build settings.
    EXPEREMENT = 15,//Values pulled from build settings.
}

public class GameManagerScript : MonoBehaviour
{
    Level currentLevel;
    [SerializeField] LevelBankScript levelBank = null;
    [SerializeField] SpeciesBank speciesBank = null;
    [SerializeField] RulesBank rulesBank = null;

    public SpeciesBank GetSpeciesBank() { return speciesBank; }

    int currentSeed;
    public int GetCurrentSeed() { return currentSeed; }

    SaveData currentSaveData;

    public SaveData GetCurrentSaveData() { return currentSaveData; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        currentSaveData = SaveDataScript.LoadSaveData();        

        if(currentSaveData == null)
        {
            speciesBank.InitializeSpeciesData();
        }
        else
        {
            speciesBank.InitializeSavedSpecies(currentSaveData);
        }

        RollNewSeed();              
    }

    public void SaveGame()
    {
        CurrentGameData currentGameData = speciesBank.GetCurrentGameDate();

        Species[] speciesArray = currentGameData.speciesInBank;
        string[] customNames = currentGameData.customNames;

        int speciesCount = speciesArray.Length;

        SerializedSpecies[] serializedSpecies = new SerializedSpecies[speciesCount];

        for(int i = 0; i < speciesCount; i++)
        {
            Species thisSpecies = speciesArray[i];
            serializedSpecies[i] = new SerializedSpecies(thisSpecies.defaultName, thisSpecies.speciesGroups, thisSpecies.color, thisSpecies.startingPopulation,
                rulesBank.GetIndexOfBirthRule(thisSpecies.birthRule), rulesBank.GetIndexOfDeathRule(thisSpecies.deathRule));
        }

        currentSaveData = SaveDataScript.SaveGame(new SaveData(serializedSpecies, customNames));
    }

    public int RollNewSeed()
    {
        currentSeed = Random.Range(0, 99999);
        return currentSeed;
    }

    public void SetSpecificSeed(int newSeed)
    {
        currentSeed = newSeed;
    }

    public void SetCurrentLevel(Level level) { currentLevel = level; }

    public Level GetCurrentLevel()
    {
        if(currentLevel == null)
        {
            return levelBank.GetRandomLevel();
        }

        return currentLevel;
    }

    public void LoadScene(SCENE scene)
    {
        SceneManager.LoadScene((int)scene);
    }
}
