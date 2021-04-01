using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Species))]
public class SpeciesSOEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        //Species species = (Species)target;

        base.OnInspectorGUI();

    }
}
