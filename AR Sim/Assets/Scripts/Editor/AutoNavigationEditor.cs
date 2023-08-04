using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AutoNavigation))]
public class AutoNavigationEditor : Editor
{

    public override void OnInspectorGUI()
    {
        AutoNavigation myTarget = (AutoNavigation)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Go To Location"))
        {
            // myTarget.GoToLocation();
        }

    }

}