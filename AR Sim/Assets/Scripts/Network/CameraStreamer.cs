using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CameraStreamer : MonoBehaviour
{
    /// <summary>
    /// Quality of video streaming
    /// </summary>
    [Range(1, 100)]
    public int quality = 50;

    /// <summary>
    /// Frequency of streaming message (FPS)
    /// </summary>
    [Range(1, 60)]
    public uint frequency = 24;

    [SerializeField] private Camera fpCam;
    [SerializeField] private Camera tpCam;
    private RenderTexture fpTex;
    private RenderTexture tpTex;

    // private RenderTexture renderTexture = null;
    private Texture2D texture2D = null;
    private float interval = 1f / 24;

    public enum CameraView
    {
        FirstPerson,
        ThirdPerson,
    }

    public CameraView cameraView = CameraView.FirstPerson;

    // Start is called before the first frame update
    void Start()
    {
        // Camera initialization
        fpTex = new RenderTexture(Screen.width, Screen.height, 24);
        tpTex = new RenderTexture(Screen.width, Screen.height, 24);

        fpTex.enableRandomWrite = true;
        tpTex.enableRandomWrite = true;

        fpCam.targetTexture = fpTex;
        tpCam.targetTexture = tpTex;
        texture2D = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        interval = 1f / frequency;
    }

    private void FixedUpdate()
    {
        if (interval > 0)
        {
            // Debug.Log(interval);
            interval -= Time.deltaTime;
        }
        else
        {
            SendTextureAsync();
            interval = 1f / frequency;
        }
    }

    public void ToggleCameraView()
    {
        if ((int)cameraView == Enum.GetValues(typeof(CameraView)).Length - 1)
        {
            cameraView = default(CameraView);
        }
        else
        {
            cameraView += 1;
        }
    }

    public void SetCameraView(CameraView a_cameraView)
    {
        cameraView = a_cameraView;
    }

    private void SendTextureAsync()
    {
        // Read screen render texture
        int width = 0, height = 0;
        
        switch (cameraView)
        {
            case CameraView.FirstPerson:
                RenderTexture.active = fpTex;
                width = fpTex.width;
                height = fpTex.height;
                break;
            case CameraView.ThirdPerson:
                RenderTexture.active = tpTex;
                width = tpTex.width;
                height = tpTex.height;
                break;
            default:
                break;
        }

        texture2D.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        RenderTexture.active = null;

        //±àÂë³Éjpg
        byte[] bytes = texture2D.EncodeToJPG(quality);

        GetComponent<TransmissionManager>().SendStreamAsync(bytes, width, height);

        // Debug.Log($"SendTextureAsync OK. Data size = {bytes.Length}");
    }
}
