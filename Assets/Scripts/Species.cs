using System.Collections.Generic;
using UnityEngine;

public enum SPECIES_GROUP
{
    NONE,
    VON_NEUMANN_4,
    MOORE_8,
    FIXED_RULES,
    RANDOM_RULES,
    FINAL_ENTRY_DO_NOT_REPLACE,
}

public class Species
{
    public Species(string defaultName, List<SPECIES_GROUP> speciesGroups, Color color, SPECIES_STARTING_POPULATION startingPopulation, STATE initialState, Rule birthRule, Rule deathRule, Rule[] otherRules)
    {
        this.defaultName = defaultName;
        this.speciesGroups = speciesGroups;
        this.color = color;
        this.startingPopulation = startingPopulation;
        this.birthRule = birthRule;
        this.deathRule = deathRule;
        this.otherRules = otherRules;
    }

    public Species(SpeciesObject speciesObject, RulesBank rulesBank, SaveData saveData)
    {
        speciesGroups = speciesObject.speciesGroups;
        startingPopulation = speciesObject.startingPopulation;
        initialState = speciesObject.initialState;
        if(speciesObject.ignoreBirthRule)
        {
            birthRule = rulesBank.GetRuleFromRuleObjectAtRuntime(null, null);
        }
        else
        { 
            if(speciesObject.birthRuleObject == null)
            {
                birthRule = rulesBank.GetRandomBirthRule(speciesGroups);
            }
            else
            {
                birthRule = rulesBank.GetRuleFromRuleObjectAtRuntime(speciesObject.birthRuleObject, speciesGroups);
            }            
        }
        if(speciesObject.ignoreDeathRule)
        {
            deathRule = rulesBank.GetRuleFromRuleObjectAtRuntime(null, null);
        }
        else
        { 
            if(speciesObject.deathRuleObject == null)
            {
                deathRule = rulesBank.GetRandomDeathRule(speciesGroups);

            } else
            {
                deathRule = rulesBank.GetRuleFromRuleObjectAtRuntime(speciesObject.deathRuleObject, speciesGroups);
            }            
        }

        int otherRulesLength = speciesObject.otherRules.Length;

        if(otherRulesLength < 1)
        {
            int otherRuleAmount = Random.Range(speciesObject.randomOtherRulesAmountRange[0], speciesObject.randomOtherRulesAmountRange[1] + 1);
            otherRules = rulesBank.GetRandomOtherRules(speciesGroups, otherRuleAmount);
        } else
        {
            otherRules = new Rule[otherRulesLength];

            for(int i = 0; i < otherRulesLength; i++)
            {
                otherRules[i] = rulesBank.GetRuleFromRuleObjectAtRuntime(speciesObject.otherRules[i], speciesGroups);
            }
        }

        // This only includes very few of the procgen features for now.

        bool isProcGen = false;

        if (speciesObject.birthRuleObject.randomizeCompareInts ||
            speciesObject.deathRuleObject.randomizeCompareInts) {
            isProcGen = true;
        }

        if (isProcGen) {
            int bX = birthRule.conditions[0].compareInts.x;
            int bY = birthRule.conditions[0].compareInts.y;
            int dX = deathRule.conditions[0].compareInts.x;
            int dY = deathRule.conditions[0].compareInts.y;
            defaultName = speciesObject.defaultName + bX + bY + dX + dY;
            bool found = false;
            if (saveData != null) {
                foreach (string name in saveData.defaultNames) {
                    if (name  == defaultName) {
                        found = true;
                    }
                }
            }
            if (!found) {
                color = Random.ColorHSV(0f, 1f, 0f, 1f, 0f, 1f, 1f, 1f);
            } else {
                if (saveData != null) {
                    for (int i = 0; i < saveData.defaultNames.Length; i++) {
                        if (saveData.defaultNames[i] == defaultName) {
                            color = new Color(saveData.speciesColors[i][0],
                                              saveData.speciesColors[i][1],
                                              saveData.speciesColors[i][2],
                                              saveData.speciesColors[i][3]);
                        }
                    }
                }
            }
        } else {
            defaultName = speciesObject.defaultName;
            color = speciesObject.color;
        }
    }

    public string defaultName;
    public List<SPECIES_GROUP> speciesGroups;
    public Color color = Color.white;
    public SPECIES_STARTING_POPULATION startingPopulation = SPECIES_STARTING_POPULATION.COMMON;
    public STATE initialState;
    public Rule birthRule;
    public Rule deathRule;
    public Rule[] otherRules;

    public override string ToString () {
        return defaultName;
    }
}
