using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeciesCollection : MonoBehaviour
{
    [SerializeField] TMP_Dropdown levelSelectDropdown = null;

    LevelBankScript levelBank = null;

    [SerializeField] GameObject gameManagerPrefab = null;

    [SerializeField] TMP_InputField seedInputField = null;

    [SerializeField] Toggle toggleHideUnknown = null;

    Level[] levels;

    string[] KnownSpeciesNames;

    int selectedSpecies = 0;

    GameManagerScript gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManagerScript>();

        if(gameManager == null)
        {
            gameManager = Instantiate(gameManagerPrefab).GetComponent<GameManagerScript>();
        }

        seedInputField.text = gameManager.GetCurrentSeed().ToString();

        levelBank = gameManager.GetComponent<LevelBankScript>();

        levels = levelBank.GetLevels();

        KnownSpeciesNames = SaveDataScript.LoadSaveData().customNames;

        List<TMP_Dropdown.OptionData> optionsDataList = new List<TMP_Dropdown.OptionData>();

        foreach (string name in KnownSpeciesNames) {
            bool isUnknown = name == "Unknown";
            if (isUnknown && toggleHideUnknown.isOn) {
                continue;
            }
            optionsDataList.Add(new TMP_Dropdown.OptionData(name));
        }

        levelSelectDropdown.AddOptions(optionsDataList);
    }

    public void RollNewSeed()
    {
        seedInputField.text = gameManager.RollNewSeed().ToString();
    }

    public void UpdateSeed()
    {
        gameManager.SetSpecificSeed(int.Parse(seedInputField.text));
    }

    public void UpdateSelectedSpecies()
    {
        selectedSpecies = levelSelectDropdown.value;
    }

    public void DeleteSaveData()
    {
        SaveDataScript.DeleteSaveData();
        gameManager.GetSpeciesBank().InitializeSpeciesData();
    }

    public void PlayLevelWithSelectedSpecies()
    {
        SaveData data = SaveDataScript.LoadSaveData();
        Level level = ScriptableObject.CreateInstance(typeof (Level)) as Level;
        level.levelWidth = 10;
        level.levelHeight = 10;
        //level.specificSpecies = ;
        //level.ruleObjects = ;
        PlayLevel(level);
    }

    public void PlayRandomLevel()
    {
        PlayLevel(Random.Range(0, levels.Length));
    }

    void PlayLevel(int levelIndex)
    {
        Random.InitState(gameManager.GetCurrentSeed());
        gameManager.SetCurrentLevel(levelBank.GetLevelByIndex(levelIndex));
        gameManager.LoadScene(SCENE.PLAY_SCREEN);
    }

    void PlayLevel (Level level) {
        Random.InitState(gameManager.GetCurrentSeed());
        gameManager.SetCurrentLevel(level);
        gameManager.LoadScene(SCENE.PLAY_SCREEN);
    }
}
