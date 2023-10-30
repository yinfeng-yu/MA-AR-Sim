using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInitializer : MonoBehaviour
{
    public Transform FPStreamCam;
    public Transform FPCam;
    public Transform TPStreamCam;
    public Transform TPCam;

    // Start is called before the first frame update
    void Start()
    {
        FPCam.position = FPStreamCam.position;
        TPCam.position = TPStreamCam.position;
    }

}
