using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct StreamDataHeader
{
    public int id;
    public int count;
    public int offset;
    public int size;
    public int totalCount;
    public int totalSize;
    public int width;
    public int height;

    public StreamDataHeader(int a_id, int a_count, int a_offset, int a_size, int a_totalCount, int a_totalSize, int a_width, int a_height)
    {
        id = a_id;
        count = a_count;
        offset = a_offset;
        size = a_size;
        totalCount = a_totalCount;
        totalSize = a_totalSize;
        width = a_width;
        height = a_height;
    }
}

// public struct StreamFrame
// {
//     public int id;
//     public int count;
//     public int offset;
//     public int size;
//     public int totalCount;
//     public int totalSize;
//     public int width;
//     public int height;
// 
//     public byte[] data;
// 
//     public StreamFrame(int a_id, int a_count, int a_offset, int a_size, int a_totalCount, int a_totalSize, int a_width, int a_height, byte[] a_data)
//     {
//         id = a_id;
//         count = a_count;
//         offset = a_offset;
//         size = a_size;
//         totalCount = a_totalCount;
//         totalSize = a_totalSize;
//         width = a_width;
//         height = a_height;
// 
//         data = a_data;
//     }
// }

public class CameraStreamer : MonoBehaviour
{
    public int width = 828;
    public int height = 472;
    /// <summary>
    /// Quality of video streaming
    /// </summary>
    [Range(1, 100)]
    public int quality = 10;

    /// <summary>
    /// Frequency of streaming message (FPS)
    /// </summary>
    [Range(1, 60)]
    public uint frequency = 24;

    [SerializeField] private Camera fpCam;
    [SerializeField] private Camera tpCam;
    [SerializeField] private Camera bvCam;

    [SerializeField] private RawImage FPRawImage;
    [SerializeField] private RawImage TPRawImage;
    [SerializeField] private RawImage BVRawImage;

    private RenderTexture fpTex;
    private RenderTexture tpTex;
    private RenderTexture bvTex;

    // private RenderTexture renderTexture = null;
    private Texture2D texture2D = null;
    private float interval = 1f / 24;

    private int streamId = 0;
    private int maxStreamId = int.MaxValue;

    public TextMeshProUGUI tmp;

    public bool isSendStream = false;

    public enum CameraView
    {
        FirstPerson,
        ThirdPerson,
        BirdsView,
    }

    public CameraView cameraView = CameraView.FirstPerson;

    // Start is called before the first frame update
    void Start()
    {
        // Camera initialization
        fpTex = new RenderTexture(width, height, 24);
        // fpTex = new RenderTexture(Screen.width, Screen.height, 24);
        tpTex = new RenderTexture(width, height, 24);
        // tpTex = new RenderTexture(Screen.width, Screen.height, 24);
        bvTex = new RenderTexture(width, height, 24);

        fpTex.enableRandomWrite = true;
        tpTex.enableRandomWrite = true;
        bvTex.enableRandomWrite = true;

        fpCam.targetTexture = fpTex;
        tpCam.targetTexture = tpTex;
        bvCam.targetTexture = bvTex;

        FPRawImage.texture = fpTex;
        TPRawImage.texture = tpTex;
        BVRawImage.texture = bvTex;

        texture2D = new Texture2D(width, height, TextureFormat.RGB24, false);

        interval = 1f / frequency;
    }

    private void FixedUpdate()
    {
        if (isSendStream)
        {
            if (interval > 0)
            {
                interval -= Time.deltaTime;
            }
            else
            {
                SendTexture();
                interval = 1f / frequency;
            }
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

    private void SendTexture()
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
            case CameraView.BirdsView:
                RenderTexture.active = bvTex;
                width = bvTex.width;
                height = bvTex.height;
                break;
            default:
                break;
        }

        texture2D.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        RenderTexture.active = null;

        // Encode to jpg
        byte[] bytes = texture2D.EncodeToJPG(quality);

        int totalSize = bytes.Length;
        int chunkSize = 2048;

        int totalCount = (totalSize / chunkSize) + (totalSize % chunkSize == 0 ? 0 : 1);
        int lastChunkSize = totalSize % chunkSize == 0 ? chunkSize : totalSize % chunkSize;

        for (int i = 0; i < totalCount; i ++)
        {
            if (i == totalCount - 1)
            {
                byte[] chunkBytes = new byte[lastChunkSize];
                Array.Copy(bytes, i * chunkSize, chunkBytes, 0, lastChunkSize);
                TransmissionManager.Instance.SendTo(new StreamMessage(new StreamDataHeader(streamId, i, i * chunkSize, lastChunkSize, totalCount, totalSize, width, height), chunkBytes), Platform.Smartphone);
                // TransmissionManager.instance.SendStream(chunkBytes, streamId, i, i * chunkSize, lastChunkSize, totalCount, totalSize, width, height);
            }
            else
            {
                byte[] chunkBytes = new byte[chunkSize];
                Array.Copy(bytes, i * chunkSize, chunkBytes, 0, chunkSize);
                TransmissionManager.Instance.SendTo(new StreamMessage(new StreamDataHeader(streamId, i, i * chunkSize, chunkSize, totalCount, totalSize, width, height), chunkBytes), Platform.Smartphone);
                // TransmissionManager.instance.SendStream(chunkBytes, streamId, i, i * chunkSize, chunkSize, totalCount, totalSize, width, height);
            }
        }

        // tmp.text = "";
        // tmp.text += $"stream data size = {bytes.Length}, quality = {quality}, width = {width}, height = {height}\n";

        TickStreamId();

        // Debug.Log($"SendTextureAsync OK. Data size = {bytes.Length}");
    }


    void TickStreamId()
    {
        if (streamId == maxStreamId) streamId = 0;
        else streamId++;
    }
}