using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Wall))]
public class WallEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Wall myTarget = (Wall)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Update Wall"))
        {
            myTarget.UpdateWall();
        }

        if (GUILayout.Button("Reset"))
        {
            myTarget.ResetWall();
        }

    }

}
