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
    GridManagerScript gridManager;

    DD_DataDiagram diagram;
    List<GameObject> lines = new List<GameObject>();

    int numberOfCells;

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
        gridManager = FindObjectOfType<GridManagerScript>();

        // CentimeterPerCoordUnitY = 1.0f -> around 7.5 cells max
        numberOfCells = gridManager.NumberOfCells;
        float scalingFactor = 1f / (numberOfCells / 7.5f);
        diagram.m_CentimeterPerCoordUnitY = scalingFactor;

        foreach (Species species in cellManager.enabledSpecies) {
            GameObject line = diagram.AddLine(species.defaultName, species.color);
            lines.Add(line);
        }
    }

    public void UpdateSpeciesPopulation(List<PopulationCount> populationReport, Dictionary<Species, int> populationDictionary)
    {
        populationBySpecies = populationDictionary;

        CalcAndDisplayAll(populationReport);
    }

    void CalcAndDisplayAll(List<PopulationCount> populationCount)
    {
        foreach (PopulationCount popCount in populationCount) {
            foreach (GameObject line in lines) {
                if (line.GetComponent<DD_Lines>().lineName == popCount.species.defaultName) diagram.InputPoint(line, new Vector2(1, popCount.population));
            }
        }
    }

    public Species GetSpeciesAtRank(int index)
    {
        return topThreePopulations[index].species;
    }

    private void Update () {
        int numCellsAlive = 0;
        foreach (KeyValuePair<Species, int> entry in populationBySpecies) {
            numCellsAlive += entry.Value;
        }
        AkSoundEngine.SetRTPCValue("NumCellsAlive", numCellsAlive, gameObject);

        int numSpeciesAlive = populationBySpecies.Count;
        AkSoundEngine.SetRTPCValue("NumSpeciesAlive", numSpeciesAlive, gameObject);

    }
}
