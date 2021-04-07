using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(RulesBank))]
public class RulesBankEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        RulesBank bankScript = (RulesBank)target;

        base.OnInspectorGUI();
        
        if(GUILayout.Button("Rebuild Rules Bank Array"))
        {
            List<RuleObject> gatheredRulesObjects = new List<RuleObject>();

            string[] guids = AssetDatabase.FindAssets("t: RuleObject", new string[] { "Assets/Scriptable Objects/Rules" });
            foreach(string guid in guids)
            {
                //Debug.Log(AssetDatabase.GUIDToAssetPath(guid));
                RuleObject thisRuleObject = (RuleObject)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(RuleObject));
                gatheredRulesObjects.Add(thisRuleObject);
            }

            bankScript.AssignRuleObjectArray(gatheredRulesObjects.ToArray());
            EditorUtility.SetDirty(bankScript.gameObject);
        }
    }
}
