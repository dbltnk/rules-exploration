using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SPECIES_GROUP
{
    NONE,
    BLOB,
    FLOPPER,
    GOBLIN,
    ROCK,
}

public class SpeciesBank : MonoBehaviour
{
    public Species[] speciesBank;

    string[] speciesCustomName;

    Dictionary<Species, int> speciesToIndex;

    public string[] InitializeSpeciesNames()
    {
        speciesToIndex = new Dictionary<Species, int>();

        int speciesCount = speciesBank.Length;

        speciesCustomName = new string[speciesCount];

        for(int i = 0; i < speciesCount; i++)
        {
            Species thisSpecies = speciesBank[i];

            if(thisSpecies == null)
            {
                continue;
            }

            speciesCustomName[i] = thisSpecies.defaultName;
            speciesToIndex[thisSpecies] = i;
        }

        return speciesCustomName;
    }

    public void SetSpeciesName(Species species, string newName)
    {
        speciesCustomName[speciesToIndex[species]] = newName;
    }

    public string GetSpeciesName(Species species)
    {
        return speciesCustomName[speciesToIndex[species]];
    }
}
