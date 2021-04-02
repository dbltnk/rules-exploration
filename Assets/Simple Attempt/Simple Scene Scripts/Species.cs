using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Species
{
    public Species(string defaultName, List<SPECIES_GROUP> speciesGroups, Color color, SPECIES_STARTING_POPULATION startingPopulation, Rule birthRule, Rule deathRule)
    {
        this.defaultName = defaultName;
        this.speciesGroups = speciesGroups;
        this.color = color;
        this.startingPopulation = startingPopulation;
        this.birthRule = birthRule;
        this.deathRule = deathRule;
    }

    public Species(SpeciesObject speciesObject)
    {
        defaultName = speciesObject.defaultName;
        speciesGroups = speciesObject.speciesGroups;
        color = speciesObject.color;
        startingPopulation = speciesObject.startingPopulation;
        birthRule = speciesObject.birthRule;
        deathRule = speciesObject.deathRule;
    }

    public string defaultName;
    public List<SPECIES_GROUP> speciesGroups;
    public Color color = Color.white;
    public SPECIES_STARTING_POPULATION startingPopulation = SPECIES_STARTING_POPULATION.COMMON;
    public Rule birthRule;
    public Rule deathRule;
}
