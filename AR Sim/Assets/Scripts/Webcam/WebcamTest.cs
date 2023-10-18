using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebcamTest : MonoBehaviour
{
    static WebCamTexture webCam;
    // Start is called before the first frame update
    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        for (var i = 0; i < devices.Length; i++)
            Debug.Log(devices[i].name);

        if (webCam == null)
        {
            webCam = new WebCamTexture();
        }
        GetComponent<Renderer>().material.mainTexture = webCam;

        if (!webCam.isPlaying)
        {
            webCam.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
