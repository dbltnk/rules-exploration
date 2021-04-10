using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

[CreateAssetMenu(fileName = "New Level", menuName = "Level")]
public class Level : ScriptableObject
{
    public string levelName => name;
    public int levelWidth = 12;
    public int levelHeight = 12;
    public Vector2Int procgenSpeciesAmount = new Vector2Int(0, 0);
    public SpeciesObject[] specificSpecies;
    public RuleObject[] ruleObjects;
}
