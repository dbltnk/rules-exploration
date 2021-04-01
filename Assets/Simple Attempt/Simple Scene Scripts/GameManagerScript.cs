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
    [SerializeField] NameManagerScript nameManager = null;

    public NameManagerScript GetNameManager() { return nameManager; }

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
            currentSaveData = new SaveData(new string[0], new string[0]);
            SaveGame();
        }

        RollNewSeed();              
    }

    public void SaveGame()
    {
        SaveDataScript.SaveGame(currentSaveData);
    }

    public void SaveGame(SaveData saveData)
    {
        currentSaveData = SaveDataScript.SaveGame(saveData);
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
