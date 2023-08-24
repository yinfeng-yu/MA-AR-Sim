using UnityEngine;
using System.Collections;
using System.Linq;
// using UnityEngine.XR.WSA.WebCam;
using UnityEngine.Windows.WebCam;
using UnityEngine.UI;

public class PhotoCaptureExample : MonoBehaviour
{
    PhotoCapture photoCaptureObject = null;
    public Texture2D targetTexture;
    public RawImage rawImage;

    // Use this for initialization
    void Start()
    {
        // Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        // targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);
        // Debug.Log("Webcam Mode is: " + WebCamMode.PhotoMode);

        int cameraResolutionWidth = 1080;
        int cameraResolutionHeight = 960;
        targetTexture = new Texture2D(cameraResolutionWidth, cameraResolutionHeight);
        
        rawImage.texture = targetTexture;
        
        // Create a PhotoCapture object
        PhotoCapture.CreateAsync(false, delegate (PhotoCapture captureObject) {
            photoCaptureObject = captureObject;
            CameraParameters cameraParameters = new CameraParameters();
            cameraParameters.hologramOpacity = 0.0f;
            // cameraParameters.cameraResolutionWidth = cameraResolution.width;
            cameraParameters.cameraResolutionWidth = cameraResolutionWidth;

            // cameraParameters.cameraResolutionHeight = cameraResolution.height;
            cameraParameters.cameraResolutionHeight = cameraResolutionHeight;
            cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

            // Activate the camera
            photoCaptureObject.StartPhotoModeAsync(cameraParameters, delegate (PhotoCapture.PhotoCaptureResult result) {
                // Take a picture
                photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
            });
        });

    }

    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        Debug.Log("d");
        // Copy the raw image data into the target texture
        photoCaptureFrame.UploadImageDataToTexture(targetTexture);
        Debug.Log("e");
        // Create a GameObject to which the texture can be applied
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Renderer quadRenderer = quad.GetComponent<Renderer>() as Renderer;
        quadRenderer.material = new Material(Shader.Find("Custom/Unlit/UnlitTexture"));

        quad.transform.parent = this.transform;
        quad.transform.localPosition = new Vector3(0.0f, 0.0f, 3.0f);

        quadRenderer.material.SetTexture("_MainTex", targetTexture);

        // Deactivate the camera
        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        // Shutdown the photo capture resource
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
    }
}