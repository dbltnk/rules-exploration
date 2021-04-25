using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeciesCollection : MonoBehaviour
{
    [SerializeField] TMP_Dropdown levelSelectDropdown = null;

    [SerializeField] GameObject gameManagerPrefab = null;

    [SerializeField] TMP_InputField seedInputField = null;

    [SerializeField] Toggle toggleHideUnknown = null;

    SaveData saveData;

    int selectedSpecies = 0;

    GameManagerScript gameManager;

    [SerializeField] Level LevelEmpty = null;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManagerScript>();

        if(gameManager == null)
        {
            gameManager = Instantiate(gameManagerPrefab).GetComponent<GameManagerScript>();
        }

        seedInputField.text = gameManager.GetCurrentSeed().ToString();

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
        string customName = levelSelectDropdown.captionText.text;
        Species speciesFound = null;
        foreach (Species species in gameManager.GetSpeciesBank().speciesBank) {
            for (int i = 0; i < saveData.customNames.Length; i++) {
                if (customName == saveData.customNames[i] && saveData.defaultNames[i] == species.defaultName) speciesFound = species;
            }
        }

        Random.InitState(gameManager.GetCurrentSeed());
        gameManager.SetCurrentLevel(LevelEmpty);
        gameManager.SpeciesFromCollection = speciesFound;
        gameManager.LoadScene(SCENE.PLAY_SCREEN);
    }
}
