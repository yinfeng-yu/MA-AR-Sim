using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoomLoader))]
public class RoomLoaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RoomLoader myTarget = (RoomLoader)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Load Room"))
        {
            myTarget.LoadRoom();
        }

        if (GUILayout.Button("Clear Room"))
        {
            myTarget.ClearRoom();
        }

    }

}
