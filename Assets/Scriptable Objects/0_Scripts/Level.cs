using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Level")]
public class Level : ScriptableObject
{
    public string levelName = "Level Name";

    public int levelWidth = 15;
    public int levelHeight = 10;

    public Species[] species;

    public Rule[] rules;
}
