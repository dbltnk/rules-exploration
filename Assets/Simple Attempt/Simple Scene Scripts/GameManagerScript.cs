using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SCENE
{
    LEVEL_SETUP = 11,//Values pulled from build settings.
    EXPEREMENT = 12,//Values pulled from build settings.
}

public class GameManagerScript : MonoBehaviour
{
    Level currentLevel;
    [SerializeField] LevelBankScript levelBank = null;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
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
