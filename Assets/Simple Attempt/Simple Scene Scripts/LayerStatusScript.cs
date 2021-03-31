using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public struct PopulationCount
{
    public PopulationCount(Species species, int population)
    {
        this.species = species;
        this.population = population;
    }

    public Species species;
    public int population;
}

public class LayerStatusScript : MonoBehaviour
{
    [SerializeField] TMP_Text[] topThreeReadout = null;

    Dictionary<Species, int> populationBySpecies;

    PopulationCount[] topThreePopulations;

    private void Awake()
    {
        populationBySpecies = new Dictionary<Species, int>();

        topThreePopulations = new PopulationCount[]
        {
            new PopulationCount(null, 0),
            new PopulationCount(null, 0),
            new PopulationCount(null, 0),
        };
    }

    public void UpdateSpeciesPopulation(List<PopulationCount> populationReport, Dictionary<Species, int> populationDictionary)
    {
        populationBySpecies = populationDictionary;

        CalcAndDisplayTopThree(populationReport);
    }

    void CalcAndDisplayTopThree(List<PopulationCount> populationCount)
    {
        PopulationCount[] newPopulationRanking = new PopulationCount[]
        {
            new PopulationCount(null, 0),
            new PopulationCount(null, 0),
            new PopulationCount(null, 0),
        };

        for(int i = 0; i < populationCount.Count; i++)
        {
            PopulationCount newEntry = populationCount[i];

            int newValue = newEntry.population;

            if(newValue > newPopulationRanking[0].population)
            {
                newPopulationRanking[2] = newPopulationRanking[1];
                newPopulationRanking[1] = newPopulationRanking[0];
                newPopulationRanking[0] = newEntry;
            }
            else if(newValue > newPopulationRanking[1].population)
            {
                newPopulationRanking[2] = newPopulationRanking[1];
                newPopulationRanking[1] = newEntry;
            }
            else if(newValue > newPopulationRanking[2].population)
            {
                newPopulationRanking[2] = newEntry;
            }            
        }

        for(int i = 0; i < newPopulationRanking.Length; i++)
        {
            PopulationCount currentPopulationCount = newPopulationRanking[i];

            if(currentPopulationCount.species == null)
            {
                topThreeReadout[i].text = "---NULL---";
            }
            else
            {
                topThreeReadout[i].text = string.Format("{0}: {1}", currentPopulationCount.species.name, currentPopulationCount.population);
            }
        }
    }

    public Species GetSpeciesAtRank(int index)
    {
        return topThreePopulations[index].species;
    }
}
