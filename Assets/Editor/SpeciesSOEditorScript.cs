using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpeciesObject))]
public class SpeciesSOEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        SpeciesObject species = (SpeciesObject)target;

        base.OnInspectorGUI();

        species.defaultName = target.name;
    }
}
