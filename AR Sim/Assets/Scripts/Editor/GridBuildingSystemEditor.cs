using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridBuildingSystem))]
public class GridBuildingSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GridBuildingSystem myTarget = (GridBuildingSystem)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Generate Grid"))
        {
            myTarget.GenerateGrid();
        }

        if (GUILayout.Button("Clear Grid"))
        {
            myTarget.ClearGrid();
        }

        if (GUILayout.Button("Save Room"))
        {
            myTarget.SaveRoom();
        }

    }

}
