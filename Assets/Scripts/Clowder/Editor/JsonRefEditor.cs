using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;

[CustomEditor(typeof(JsonRef))]
public class JsonRefEditor : Editor
{


    public override void OnInspectorGUI()
    {

        JsonRef component = (JsonRef)target;

        //base.OnInspectorGUI();
        DrawDefaultInspector();


        
        if (component.StaticJson != null)
        {
            //GUILayout.TextArea(component.StaticJson.text);
            // GUILayout.TextArea
            component.setJsonFromAsset();
        }
        
        
        if (component.BlobJson != null)
        {
            GUILayout.Label("Uploaded File JSON");
            GUILayout.TextArea(component.BlobJson.ToString(true), GUILayout.MaxHeight(75));
            /*
            if (GUILayout.Button("Post to Clowder"))
            {
                component.postMetaData(component.JsonFile);
            }*/
        }
        if (component.MetaJson != null)
        {
            GUILayout.Label("Technical Metadata JSON");
            GUILayout.TextArea(component.MetaJson.ToString(true), GUILayout.MaxHeight(75));
        }

        if (GUILayout.Button("Serialize Fields"))
        {
            component.SerializeFields();
            //component.postMetaData(component.JsonFile);
        }

        if (component.JsonOutput != null)
        {
            GUILayout.Label("Self-Serialized JSON");
            GUILayout.TextArea(component.JsonOutput.ToString(true), GUILayout.MaxHeight(75));
            if (GUILayout.Button("Post to Clowder"))
            {
                component.postMetaData(component.JsonOutput);
            }
        }

        
        

        //component.fileid = 
        //component.onSprite = (Sprite)EditorGUILayout.ObjectField("On Sprite", component.onSprite, typeof(Sprite), true);
        //component.onTextColor = EditorGUILayout.ColorField("On text colour", component.onTextColor);
        //component.offSprite = (Sprite)EditorGUILayout.ObjectField("Off Sprite", component.offSprite, typeof(Sprite), true);
        //component.offTextColor = EditorGUILayout.ColorField("Off text colour", component.offTextColor);

    }

    
}