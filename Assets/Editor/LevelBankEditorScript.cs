using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(LevelBankScript))]
public class LevelBankEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        LevelBankScript bankScript = (LevelBankScript)target;

        base.OnInspectorGUI();
        
        if(GUILayout.Button("Rebuild Level Bank Array"))
        {
            List<Level> gatheredLevels = new List<Level>();

            string[] guids = AssetDatabase.FindAssets("t: Level", new string[] { "Assets/Scriptable Objects/Levels" });
            foreach(string guid in guids)
            {
                Level thisLevel = (Level)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(Level));
                gatheredLevels.Add(thisLevel);
            }

            bankScript.AssignLevels(gatheredLevels.ToArray());
            EditorUtility.SetDirty(bankScript.gameObject);
        }
    }
}
