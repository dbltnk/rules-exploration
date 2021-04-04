using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Level")]
public class Level : ScriptableObject
{
    public string levelName = "Level Name";

    public int levelWidth = 15;
    public int levelHeight = 10;

    public Vector2Int procgenSpeciesAmount = new Vector2Int(1, 2);

    public SpeciesObject[] specificSpecies;

    public Rule[] rules;
}
