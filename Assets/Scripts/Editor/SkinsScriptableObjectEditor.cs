using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.UIElements;
using UnityEngine.UIElements;


[CustomEditor(typeof(SkinsScriptableObject))]
public class SkinsScriptableObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var script = (SkinsScriptableObject)target;

        if (GUILayout.Button("Regenerate empty keys", GUILayout.Height(20)))
        {
            script.RegenerateAllEmptyKeys();
        }

        base.OnInspectorGUI();
    }
}


