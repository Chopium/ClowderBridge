using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(masterSwitchInterface))]
public class masterSwitchInterface_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        masterSwitchInterface myScript = (masterSwitchInterface)target;
        if (GUILayout.Button("Post Settings"))
        {
            myScript.postSettings();
        }
        if (GUILayout.Button("Update Settings"))
        {
            myScript.updateSettings();
        }
        if (GUILayout.Button("Fetch Settings"))
        {
            myScript.fetchSettings();
        }
        if (GUILayout.Button("Delete Settings"))
        {
            myScript.deleteAllSettings();
        }
        if (GUILayout.Button("Apply Settings"))
        {
            myScript.updateSettings();
        }
        if (GUILayout.Button("Delete Ornaments"))
        {
            Ornament_Master.Instance.deleteAllOrnaments();
        }
        if (GUILayout.Button("Update Ornaments"))
        {
            Ornament_Master.Instance.fetchOrnaments();
        }

    }

}