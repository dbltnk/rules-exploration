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
    /// <summary>
    /// The species will not gain a birth rule.
    /// </summary>
    public bool ignoreBirthRule = false;
    /// <summary>
    /// Leave empty for a random birth rule.
    /// </summary>
    public RuleObject birthRuleObject;
    /// <summary>
    /// The species will not gain a death rule.
    /// </summary>
    public bool ignoreDeathRule = false;
    /// <summary>
    /// Leave empty for a random death rule.
    /// </summary>
    public RuleObject deathRuleObject;
    /// <summary>
    /// If Other Rules arrau is null, generate an amount of other rules between these amount.
    /// </summary>
    public Vector2Int randomOtherRulesAmountRange = new Vector2Int(0, 1);
    /// <summary>
    /// Leave empty for random other rules.
    /// </summary>
    public RuleObject[] otherRules;
}
