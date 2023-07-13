using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    static int maskOff;
    static bool cameraOn = false;

    public static void SwitchCamera()
    {
        if (!cameraOn)
        {
            maskOff = Camera.main.cullingMask;
            Camera.main.cullingMask = -1;
        }
        else
        {
            Camera.main.cullingMask = maskOff;
        }
    }
}
