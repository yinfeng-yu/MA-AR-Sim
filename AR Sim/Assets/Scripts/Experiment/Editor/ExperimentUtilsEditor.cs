using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ExperimentUtils))]
public class ExperimentUtilsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ExperimentUtils myTarget = (ExperimentUtils)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Reset"))
        {
            myTarget.ResetScene();
        }

        if (GUILayout.Button("Start Move Task"))
        {
            myTarget.StartMoveTask();
        }

        if (GUILayout.Button("Start Switch Task"))
        {
            myTarget.StartSwitchTask();
        }

        if (GUILayout.Button("Start Grab Task"))
        {
            myTarget.StartGrabTask();
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
