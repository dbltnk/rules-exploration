using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelSetupScript : MonoBehaviour
{
    [SerializeField] TMP_Dropdown levelSelectDropdown = null;

    LevelBankScript levelBank = null;

    [SerializeField] GameObject gameManagerPrefab = null;

    Level[] levels;

    int selectedLevel = 0;

    GameManagerScript gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManagerScript>();

        if(gameManager == null)
        {
            gameManager = Instantiate(gameManagerPrefab).GetComponent<GameManagerScript>();
        }

        levelBank = gameManager.GetComponent<LevelBankScript>();

        levels = levelBank.GetLevels();

        List<TMP_Dropdown.OptionData> optionsDataList = new List<TMP_Dropdown.OptionData>();

        for(int i = 0; i < levels.Length; i++)
        {
            Level thisLevel = levels[i];

            optionsDataList.Add(new TMP_Dropdown.OptionData(thisLevel.levelName));
        }

        levelSelectDropdown.AddOptions(optionsDataList);
    }

    public void UpdateSelectedLevel()
    {
        selectedLevel = levelSelectDropdown.value;
    }

    public void PlaySelectedLevel()
    {
        PlayLevel(selectedLevel);
    }

    public void PlayRandomLevel()
    {
        PlayLevel(Random.Range(0, levels.Length));
    }

    void PlayLevel(int levelIndex)
    {
        gameManager.SetCurrentLevel(levelBank.GetLevelByIndex(levelIndex));
        gameManager.LoadScene(SCENE.EXPEREMENT);
    }
}
