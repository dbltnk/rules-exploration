using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SCENE
{
    LEVEL_SETUP = 14,//Values pulled from build settings.
    EXPERIMENT = 15,//Values pulled from build settings.
}

public class GameManagerScript : MonoBehaviour
{
    Level currentLevel;
    [SerializeField] LevelBankScript levelBank = null;
    [SerializeField] SpeciesBank speciesBank = null;
    [SerializeField] RulesBank rulesBank = null;

    [SerializeField] bool DEBUG_ALWAYS_DELETE_SAVE_DATA = false;

    public SpeciesBank GetSpeciesBank() { return speciesBank; }

    int currentSeed;
    public int GetCurrentSeed() { return currentSeed; }

    SaveData currentSaveData;

    public SaveData GetCurrentSaveData() { return currentSaveData; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        currentSaveData = SaveDataScript.LoadSaveData();        

        if(currentSaveData == null ||
            DEBUG_ALWAYS_DELETE_SAVE_DATA)
        {
            CreateNewGameSave();
        }
        else
        {
            LoadSavedGame();   
        }

        RollNewSeed();              
    }

    public void LoadSavedGame()
    {
        rulesBank.LoadSavedRuleBank(currentSaveData);
        speciesBank.InitializeSavedSpecies(currentSaveData);
    }

    public void CreateNewGameSave()
    {
        rulesBank.InitializeNewRulesBank();
        speciesBank.InitializeSpeciesData();
    }

    public void SaveGame()
    {
        CurrentGameData currentGameData = speciesBank.GetCurrentGameDate();

        Species[] speciesArray = currentGameData.speciesInBank;
        string[] customNames = currentGameData.customNames;
        Rule[] rulesArray = currentGameData.rulesInBank;

        int speciesCount = speciesArray.Length;

        SerializedSpecies[] serializedSpecies = new SerializedSpecies[speciesCount];

        for(int i = 0; i < speciesCount; i++)
        {            
            Species thisSpecies = speciesArray[i];
            serializedSpecies[i] = new SerializedSpecies(thisSpecies.defaultName, thisSpecies.speciesGroups, thisSpecies.color, thisSpecies.startingPopulation,
                thisSpecies.birthRule.ruleIndex, thisSpecies.deathRule.ruleIndex, thisSpecies.otherRules);
        }

        int rulesCount = rulesArray.Length;

        SerializedRule[] serializeRules = new SerializedRule[rulesCount];

        for(int i = 0; i < rulesCount; i++)
        {
            Rule thisRule = rulesArray[i];

            serializeRules[i] = new SerializedRule(rulesArray[i]);
        }

        currentSaveData = SaveDataScript.SaveGame(new SaveData(serializedSpecies, customNames, serializeRules));
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
