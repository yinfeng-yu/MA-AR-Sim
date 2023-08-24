using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;

public enum CameraType
{
    MainMenu,
    FirstPerson,
    ThirdPerson,
}

[Serializable]
public struct CameraInfo
{
    public Camera camera;
    public CameraType cameraType;
    public Quaternion initOrientation;
}

public class CameraManager : MonoBehaviour
{
    public List<CameraInfo> cameraInfos;

    Dictionary<CameraType, CameraInfo> cameraIndex;

    public Camera fPStreamCamera;
    public Camera tPStreamCamera;

    Camera currentCamera;

    private void Start()
    {
        cameraIndex = new Dictionary<CameraType, CameraInfo>();
        foreach (CameraInfo camInfo in cameraInfos)
        {
            CameraInfo tempCamInfo = camInfo;
            tempCamInfo.initOrientation = camInfo.camera.transform.rotation;
            cameraIndex.Add(camInfo.cameraType, tempCamInfo);
        }

        ActivateCamera(ControlMode.MainMenu);
    }

    private void Update()
    {
        // if (currentCamera == mainMenuCamera)
        // {
        //     currentCamera.transform.position = Camera.main.transform.position;
        // }

        // currentCamera.transform.rotation = Camera.main.transform.rotation;
        fPStreamCamera.transform.rotation = cameraIndex[CameraType.FirstPerson].camera.transform.rotation;
        tPStreamCamera.transform.rotation = cameraIndex[CameraType.ThirdPerson].camera.transform.rotation;
    }

    private void OnDestroy()
    {
        ActivateCamera(ControlMode.MainMenu);
        foreach (CameraInfo camInfo in cameraInfos)
        {
            camInfo.camera.transform.rotation = cameraIndex[camInfo.cameraType].camera.transform.rotation;
        }
    }

    void CameraOrientaionReset()
    {
        foreach (CameraInfo camInfo in cameraInfos)
        {
            if (camInfo.camera != currentCamera)
            {
                camInfo.camera.transform.rotation = cameraIndex[camInfo.cameraType].initOrientation; // We can use Lerp.
            }
        }
    }

    public void ActivateCamera(ControlMode controlMode)
    {
        switch (controlMode)
        {
            case ControlMode.MainMenu:
                GetMainMenuCamera().tag = "MainCamera";
                GetFirstPersonCamera().tag = "Untagged";
                GetThirdPersonCamera().tag = "Untagged";
                CameraCache.UpdateCachedMainCamera(GetMainMenuCamera());

                currentCamera = GetMainMenuCamera();
                CameraOrientaionReset();

                GetMainMenuCamera().enabled = true;
                GetFirstPersonCamera().enabled = false;
                GetThirdPersonCamera().enabled = false;

                // GetMainMenuCamera().gameObject.GetComponent<TrackedPoseDriver>().enabled = true;
                // GetFirstPersonCamera().gameObject.GetComponent<TrackedPoseDriver>().enabled = false;
                // GetThirdPersonCamera().gameObject.GetComponent<TrackedPoseDriver>().enabled = false;
                break;

            case ControlMode.Smartphone:
                GetMainMenuCamera().tag = "Untagged";
                GetFirstPersonCamera().tag = "MainCamera";
                GetThirdPersonCamera().tag = "Untagged";
                CameraCache.UpdateCachedMainCamera(GetFirstPersonCamera());

                currentCamera = GetFirstPersonCamera();
                CameraOrientaionReset();

                GetMainMenuCamera().enabled = false;
                GetFirstPersonCamera().enabled = true;
                GetThirdPersonCamera().enabled = false;

                break;

            case ControlMode.HandTracking:
                GetMainMenuCamera().tag = "Untagged";
                GetFirstPersonCamera().tag = "MainCamera";
                GetThirdPersonCamera().tag = "Untagged";
                CameraCache.UpdateCachedMainCamera(GetFirstPersonCamera());

                currentCamera = GetFirstPersonCamera();
                CameraOrientaionReset();

                GetMainMenuCamera().enabled = false;
                GetFirstPersonCamera().enabled = true;
                GetThirdPersonCamera().enabled = false;
                break;

            default:
                break;

        }
    }

    Camera GetMainMenuCamera() => cameraIndex[CameraType.MainMenu].camera;
    Camera GetFirstPersonCamera() => cameraIndex[CameraType.FirstPerson].camera;
    Camera GetThirdPersonCamera() => cameraIndex[CameraType.ThirdPerson].camera;
}
