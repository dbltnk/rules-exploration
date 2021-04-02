using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpeciesObject))]
public class SpeciesSOEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        //Species species = (Species)target;

        base.OnInspectorGUI();

    }
}
