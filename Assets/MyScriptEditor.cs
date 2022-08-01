using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Game_of_live))]
public class MyScriptEditor : Editor
{
    private Game_of_live script;

    private void OnEnable()
    {
        script = target as Game_of_live;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Redraw", GUILayout.Width(120)))
            script.redraw();
    }
}
