using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SPECIES_STARTING_POPULATION
{
    NONE,
    VERY_RARE,
    RARE,
    UNCOMMON,
    COMMON,
    VERY_COMMON,
    UBIQUITOUS,
}

[CreateAssetMenu(fileName = "New Species", menuName = "Species")]
public class SpeciesObject : ScriptableObject
{
    public string defaultName;
    public List<SPECIES_GROUP> speciesGroups;
    public Color color = Color.white;
    public SPECIES_STARTING_POPULATION startingPopulation = SPECIES_STARTING_POPULATION.COMMON;
    public RuleObject birthRuleObject;
    public RuleObject deathRuleObject;
    public bool treatWallsAsAlive;
}
