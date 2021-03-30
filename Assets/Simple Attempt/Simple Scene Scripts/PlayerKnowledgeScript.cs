using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGameData
{
    public SaveGameData()
    {

    }


}

public class SpeciesData
{
    public SpeciesData()
    {

    }

    public string speciesName;
    public Rule[] speciesRules;
    public Rule[] speciesPropigationRules;
    public Color speciesColor;
}

public class PlayerKnowledgeScript : MonoBehaviour
{
    SaveGameData currentGameData;

    public void SaveGame()
    {

    }

    public void LoadGame()
    {
        
    }
}
