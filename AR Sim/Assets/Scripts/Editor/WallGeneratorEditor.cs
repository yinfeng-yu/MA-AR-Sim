using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WallGeneratorEditor))]
public class WallGeneratorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        WallGenerator myTarget = (WallGenerator)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Switch Edit Mode"))
        {
            myTarget.SwitchEditMode();
        }

    }

}
