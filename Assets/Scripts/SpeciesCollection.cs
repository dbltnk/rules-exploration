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

    SaveData saveData;

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

        saveData = SaveDataScript.LoadSaveData();

        List<TMP_Dropdown.OptionData> optionsDataList = new List<TMP_Dropdown.OptionData>();

        foreach (Species species in gameManager.GetSpeciesBank().speciesBank) {
            string customName = "";
            for (int i = 0; i < saveData.customNames.Length; i++) {
                if (species.defaultName == saveData.defaultNames[i]) customName = saveData.customNames[i];
            }
            bool isUnknown = customName == "Unknown";
            if (isUnknown && toggleHideUnknown.isOn) {
                continue;
            }
            optionsDataList.Add(new TMP_Dropdown.OptionData(customName));
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
        string customName = levelSelectDropdown.itemText.text;
        List<Species> speciesFound = new List<Species>();

        foreach (Species species in gameManager.GetSpeciesBank().speciesBank) {
            for (int i = 0; i < saveData.customNames.Length; i++) {
                if (customName == saveData.customNames[i]) speciesFound.Add(species);
            }
        }
        //level.specificSpecies = speciesFound.ToArray(); // Species != SpeciesObject
        //level.ruleObjects = speciesFound[0].birthRule + speciesFound[0].deathRule + speciesFound[0].otherRules; // Rule != RulesObject ... and of course this is now how you concatenate an array
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
