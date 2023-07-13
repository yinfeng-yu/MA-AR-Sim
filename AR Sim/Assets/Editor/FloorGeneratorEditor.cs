using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FloorGenerator))]
public class FloorGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        FloorGenerator myTarget = (FloorGenerator)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Generate Floor"))
        {
            myTarget.GenerateFloor();
        }

        if (GUILayout.Button("Clear Floor"))
        {
            myTarget.ClearFloor();
        }

    }

}
