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

    SpeciesBank speciesBank;
    CellManagerScript cellManager;

    DD_DataDiagram diagram;
    List<GameObject> lines = new List<GameObject>();

    public void AssignSpeciesBank(SpeciesBank speciesBank) { this.speciesBank = speciesBank; }

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
    private void Start () {
        diagram = FindObjectOfType<DD_DataDiagram>();
        cellManager = FindObjectOfType<CellManagerScript>();

        foreach (Species species in cellManager.enabledSpecies) {
            GameObject line = diagram.AddLine(species.defaultName, species.color);
            lines.Add(line);
        }

        // TODO Make the Y axis scale with the number of cells in the grid.
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

        for (int i = 0; i < newPopulationRanking.Length; i++)
        {
            PopulationCount currentPopulationCount = newPopulationRanking[i];

            if(currentPopulationCount.species == null)
            {
                topThreeReadout[i].text = "---NULL---";
                // TODO Add zeros when we do not have any members of a species
                // foreach (GameObject line in lines) {
                //     if (line.GetComponent<DD_Lines>().lineName == currentPopulationCount.species.defaultName) diagram.InputPoint(line, new Vector2(1, 0));
                // }
            }
            else
            {
                // TODO Make this update for all species in the level, not just the top three.
                topThreeReadout[i].text = string.Format("{0}: {1}", speciesBank.GetSpeciesName(currentPopulationCount.species), currentPopulationCount.population);
                foreach (GameObject line in lines) {
                    if (line.GetComponent<DD_Lines>().lineName == currentPopulationCount.species.defaultName) diagram.InputPoint(line, new Vector2(1, currentPopulationCount.population));
                }
            }
        }
    }

    public Species GetSpeciesAtRank(int index)
    {
        return topThreePopulations[index].species;
    }
}
