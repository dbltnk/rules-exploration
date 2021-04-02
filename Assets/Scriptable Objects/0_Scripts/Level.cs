using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BORDER_TYPE
{
    NORMAL,
    ALIVE,
    /// <summary>
    /// Will randomly select one of the border types at the start of the level.
    /// </summary>
    RANDOM,//Must be the last on the list.
}

[CreateAssetMenu(fileName = "New Level", menuName = "Level")]
public class Level : ScriptableObject
{
    public string levelName = "Level Name";

    public int levelWidth = 15;
    public int levelHeight = 10;

    public Vector2Int procgenSpeciesAmount = new Vector2Int(1, 2);

    public SpeciesObject[] specificSpecies;

    public Rule[] rules;

    public BORDER_TYPE borderType = BORDER_TYPE.NORMAL;
}
