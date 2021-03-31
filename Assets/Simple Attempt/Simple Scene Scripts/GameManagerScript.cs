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

    int currentSeed;
    public int GetCurrentSeed() { return currentSeed; }

    SaveData currentSaveData;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        currentSaveData = SaveDataScript.LoadSaveData();

        if(currentSaveData == null)
        {
            currentSaveData = new SaveData(speciesBank.InitializeSpeciesNames());
        }

        RollNewSeed();              
    }

    public void SaveGame()
    {
        SaveDataScript.SaveGame(currentSaveData);
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
