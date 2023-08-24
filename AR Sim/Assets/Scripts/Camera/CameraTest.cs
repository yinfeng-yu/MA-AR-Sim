using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap
{
    public class CameraTest : MonoBehaviour
    {
        static WebCamTexture backCam;
        // Start is called before the first frame update
        void Start()
        {
            // if (backCam == null)
            // {
            //     backCam = new WebCamTexture();
            // }
            // 
            // GetComponent<Renderer>().material.mainTexture = backCam;
            // 
            // if (!backCam.isPlaying)
            // {
            //     backCam.Play();
            // }

            WebCamDevice[] devices = WebCamTexture.devices;
            Debug.Log($"{devices.Length} webcam device(s) detected.");
            foreach (var d in devices)
            {
                Debug.Log(d.name.ToString());
            }

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
