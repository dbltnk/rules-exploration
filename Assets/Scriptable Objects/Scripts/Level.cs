using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SpeciesWeight
{
    public SpeciesWeight(SPECIES species, int weight)
    {
        this.species = species;
        this.weight = weight;
    }

    public SPECIES species;
    public int weight;
}

[CreateAssetMenu(fileName = "New Level", menuName = "Level")]
public class Level : ScriptableObject
{
    public string levelName = "Level Name";

    public int levelWidth = 15;
    public int levelHeight = 10;

    public SPECIES[] enabledSpecies;

    public SpeciesWeight[] speciesWeightArray;

    public PREMADE_RULES[] premadeRules;

    public RuleObject[] ruleObjects;//For if you would rather write rules in object form. They should function the same.
}
