using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Level))]
public class LevelSOEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        //Level level = (Level)target;

        base.OnInspectorGUI();

        //level.levelName = GUILayout.TextField(level.levelName);
        //target.name = level.levelName;

        //level.enabledSpecies = (SPECIES)EditorGUILayout.EnumFlagsField(level.enabledSpecies);
    }
}
