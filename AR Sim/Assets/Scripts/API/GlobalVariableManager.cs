using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariableManager : Singleton<GlobalVariableManager>
{
    // Global Variables
    public Dictionary<string, Vector3> GlobalVector3s = new Dictionary<string, Vector3>();
    public Dictionary<string, Quaternion> GlobalQuaternions = new Dictionary<string, Quaternion>();
    public Dictionary<string, float> GlobalFloats = new Dictionary<string, float>();

    [TextArea(1, 10)]
    public string HandLandmarksRaw = "";

    public Vector3 GetVector3(string label)
    {
        return GlobalVector3s[label];
    }

    public Quaternion GetQuaternion(string label)
    {
        return GlobalQuaternions[label];
    }

    public float GetFloat(string label)
    {
        return GlobalFloats[label];
    }
}
