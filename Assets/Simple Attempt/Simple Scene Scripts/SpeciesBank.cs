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
    FINAL_ENTRY_DO_NOT_REPLACE,
}

public struct CurrentGameData
{
    public CurrentGameData(Species[] speciesInBank, string[] customNames)
    {
        this.speciesInBank = speciesInBank;
        this.customNames = customNames;
    }

    public Species[] speciesInBank;
    public string[] customNames;
}

public class SpeciesBank : MonoBehaviour
{
    [SerializeField] RulesBank rulesBank = null;

    Species[] speciesBank;

    string[] speciesCustomName;

    Dictionary<string, int> speciesToIndex;

    public void InitializeSpeciesData()
    {
        speciesBank = new Species[0];
        speciesCustomName = new string[0];
        speciesToIndex = new Dictionary<string, int>();
    }

    public void InitializeSavedSpecies(SaveData saveData)
    {
        speciesToIndex = new Dictionary<string, int>();

        string[] dataDefaultNames = saveData.defaultNames;
        speciesCustomName = saveData.customNames;

        speciesBank = new Species[dataDefaultNames.Length];

        for(int i = 0; i < dataDefaultNames.Length; i++)
        {
            Species thisSpecies = DeserializeSpecies(saveData.defaultNames[i],
                saveData.speciesGroups[i],
                saveData.speciesColors[i],
                saveData.startingPopulations[i],
                saveData.birthRuleIndex[i],
                saveData.deathRuleIndex[i]);
            if(thisSpecies == null) { continue; }

            speciesBank[i] = thisSpecies;
            speciesToIndex[dataDefaultNames[i]] = i;
        }        
    }

    public List<Species> GetKnownSpecies(int amountToGet, List<string> excludedSpeciesList)
    {
        List<Species> speciesAvailable = new List<Species>();

        speciesAvailable.AddRange(speciesBank);

        List<Species> chosenSpeciesList = new List<Species>();

        for(int i = 0; i < amountToGet; i++)
        {
            if(speciesAvailable.Count < 1)
            {
                break;
            }

            int chosenIndex = Random.Range(0, speciesAvailable.Count);

            Species chosenSpecies = speciesAvailable[chosenIndex];

            if(excludedSpeciesList.Contains(chosenSpecies.defaultName))
            {
                continue;
            }

            chosenSpeciesList.Add(chosenSpecies);
            speciesAvailable.RemoveAt(chosenIndex);
        }

        AddSpecies(chosenSpeciesList.ToArray());

        return chosenSpeciesList;
    }

    public Species[] GenerateNewSpecies(int amountToGenerate)
    {
        Species[] newSpeciesArray = new Species[amountToGenerate];

        for(int i = 0; i < amountToGenerate; i++)
        {
            List<SPECIES_GROUP> speciesGroup = new List<SPECIES_GROUP>();
            List<SPECIES_GROUP> unclaimedSpeciesGroups = new List<SPECIES_GROUP>();
            for(int s = 1; s < (int)SPECIES_GROUP.FINAL_ENTRY_DO_NOT_REPLACE; s++)
            {
                unclaimedSpeciesGroups.Add((SPECIES_GROUP)s);
            }

            int speciesGroupCount = 1;

            if(Random.Range(0, 10) == 9)
            {
                speciesGroupCount = 2;
            }

            for(int s = 0; s < speciesGroupCount; s++)
            {
                int chosenIndex = Random.Range(0, unclaimedSpeciesGroups.Count);

                speciesGroup.Add(unclaimedSpeciesGroups[chosenIndex]);
                unclaimedSpeciesGroups.RemoveAt(chosenIndex);
            }

            Color color = new Color(Random.Range(0f, 1.0f), Random.Range(0f, 1.0f), Random.Range(0f, 1.0f), 1);

            SPECIES_STARTING_POPULATION startingPopulation = (SPECIES_STARTING_POPULATION)Random.Range(1, (int)SPECIES_STARTING_POPULATION.UBIQUITOUS + 1);

            string defaultName = string.Format("{0}_{1}_{2}_{3}", speciesGroup[0].ToString(), startingPopulation.ToString(), System.DateTime.Now.ToString(), i);
            Species newSpecies = new Species(defaultName, speciesGroup, color, startingPopulation, rulesBank.GetRandomBirthRule(), rulesBank.GetRandomDeathRule());
        }

        AddSpecies(newSpeciesArray);

        return newSpeciesArray;
    }

    public CurrentGameData GetCurrentGameDate()
    {
        return new CurrentGameData(speciesBank, speciesCustomName);
    }

    Species DeserializeSpecies(string defaultName, int[] speciesGroups, float[] color, int startingPopulation, int birthRuleIndex, int deathRuleIndex)
    {
        List<SPECIES_GROUP> speciesGroupList = new List<SPECIES_GROUP>();
        for(int i = 0; i < speciesGroups.Length; i++)
        {
            speciesGroupList.Add((SPECIES_GROUP)speciesGroups[i]);
        }

        return new Species(defaultName, speciesGroupList, new Color(color[0], color[1], color[2], color[3]), (SPECIES_STARTING_POPULATION)startingPopulation,
            rulesBank.GetBirthRule(birthRuleIndex), rulesBank.GetDeathRule(deathRuleIndex));
    }

    public void AddSpecies(Species[] speciesArray)
    {
        int currentIndex = speciesBank.Length;

        List<Species> speciesToAdd = new List<Species>();
        List<string> namesToAdd = new List<string>();

        for(int i = 0; i < speciesArray.Length; i++)
        {
            Species thisSpecies = speciesArray[i];
            string defaultName = thisSpecies.defaultName;
            if(speciesToIndex.ContainsKey(defaultName)) { continue; }
            speciesToAdd.Add(thisSpecies);
            namesToAdd.Add("Unknown");            
            currentIndex++;
        }

        int newArraySize = speciesBank.Length + speciesToAdd.Count;

        Species[] newBank = new Species[newArraySize];
        string[] newNameArray = new string[newArraySize];
        
        for(int i = 0; i < speciesBank.Length; i++)
        {
            Species thisSpecies = speciesBank[i];
            newBank[i] = thisSpecies;
            newNameArray[i] = speciesCustomName[i];
            speciesToIndex[thisSpecies.defaultName] = i;
        }

        currentIndex = speciesBank.Length;

        for(int i = 0; i < speciesToAdd.Count; i++)
        {
            Species thisSpecies = speciesToAdd[i];
            newBank[currentIndex] = thisSpecies;
            newNameArray[currentIndex] = namesToAdd[i];
            speciesToIndex[thisSpecies.defaultName] = currentIndex;
            currentIndex++;
        }

        speciesBank = newBank;
        speciesCustomName = newNameArray;
    }

    public void SetSpeciesName(Species species, string newName)
    {
        speciesCustomName[speciesToIndex[species.defaultName]] = newName;
    }

    public string GetSpeciesName(Species species)
    {
        return speciesCustomName[speciesToIndex[species.defaultName]];
    }
}
