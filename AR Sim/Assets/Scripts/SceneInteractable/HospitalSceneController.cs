using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// since the scene is generated as prefab at first, adding this script to get better accessbility to the scene
/// </summary>
public class HospitalSceneController : MonoBehaviour
{
    /// <summary>
    /// Current room camera
    /// </summary>
    [System.NonSerialized] public Camera _currentRoomCam;
}
