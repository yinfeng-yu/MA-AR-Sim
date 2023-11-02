using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(Debugger))]
public class DebuggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Debugger myTarget = (Debugger)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Comfirm"))
        {
            // myTarget.Confirm();
        }

        // if (GUILayout.Button("Clear Grid"))
        // {
        //     myTarget.ClearGrid();
        // }
        // 
        // if (GUILayout.Button("Save Room"))
        // {
        //     myTarget.SaveRoom();
        // }

    }

}
