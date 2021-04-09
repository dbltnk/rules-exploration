using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

[CreateAssetMenu(fileName = "New Level", menuName = "Level")]
public class Level : ScriptableObject
{
    public string levelName = "Level Name";

    public int levelWidth = 15;
    public int levelHeight = 10;

    public Vector2Int procgenSpeciesAmount = new Vector2Int(1, 2);

    public SpeciesObject[] specificSpecies;

    public RuleObject[] ruleObjects;

    private LevelBankScript bankScript; 

    public void Awake () {
        bankScript = Resources.FindObjectsOfTypeAll<LevelBankScript>().FirstOrDefault();
        List<Level> gatheredLevels = new List<Level>();
        gatheredLevels.AddRange(bankScript.GetLevels());
        bool found = false;
        foreach (Level l in gatheredLevels) {
            if (l == this) found = true;
        }
        if (!found) gatheredLevels.Add(this);
        bankScript.AssignLevels(gatheredLevels.ToArray());
        EditorUtility.SetDirty(bankScript.gameObject);
    }

    //Disabled because Unity never calls this. Likely a bug in the engine.
    //public void OnDestroy () {
    //    List<Level> gatheredLevels = new List<Level>();
    //    gatheredLevels.AddRange(bankScript.GetLevels());
    //    for (int i = 0; i < gatheredLevels.Count; i++) {
    //        if (gatheredLevels[i] == this) gatheredLevels.Remove(gatheredLevels[i]);
    //    }
    //    bankScript.AssignLevels(gatheredLevels.ToArray());
    //    EditorUtility.SetDirty(bankScript.gameObject);
    //}
}
