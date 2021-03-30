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
}
