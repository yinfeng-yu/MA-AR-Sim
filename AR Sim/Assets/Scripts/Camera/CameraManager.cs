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
    BirdsView,
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

    Dictionary<CameraType, CameraInfo> cameraDic;

    public Camera fPStreamCamera;
    public Camera tPStreamCamera;

    Camera currentCamera;

    private void Start()
    {
        cameraDic = new Dictionary<CameraType, CameraInfo>();
        foreach (CameraInfo camInfo in cameraInfos)
        {
            CameraInfo tempCamInfo = camInfo;
            tempCamInfo.initOrientation = camInfo.camera.transform.rotation;
            cameraDic.Add(camInfo.cameraType, tempCamInfo);
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
        fPStreamCamera.transform.rotation = GetFirstPersonCamera().transform.rotation;
        tPStreamCamera.transform.rotation = GetThirdPersonCamera().transform.rotation;

        if (currentCamera != GetMainMenuCamera())
        {
            GetMainMenuCamera().transform.rotation = currentCamera.transform.rotation;
        }
    }

    private void OnDestroy()
    {
        // ActivateCamera(ControlMode.MainMenu);
        // foreach (CameraInfo camInfo in cameraInfos)
        // {
        //     camInfo.camera.transform.rotation = cameraIndex[camInfo.cameraType].camera.transform.rotation;
        // }
    }

    void CameraOrientaionReset()
    {
        foreach (CameraInfo camInfo in cameraInfos)
        {
            if (camInfo.camera != currentCamera)
            {
                camInfo.camera.transform.rotation = cameraDic[camInfo.cameraType].initOrientation; // We can use Lerp.
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

                GetMainMenuCamera().gameObject.GetComponent<TrackedPoseDriver>().enabled = true;
                GetFirstPersonCamera().gameObject.GetComponent<TrackedPoseDriver>().enabled = false;
                GetThirdPersonCamera().gameObject.GetComponent<TrackedPoseDriver>().enabled = false;
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

                GetMainMenuCamera().gameObject.GetComponent<TrackedPoseDriver>().enabled = false;
                GetFirstPersonCamera().gameObject.GetComponent<TrackedPoseDriver>().enabled = true;
                GetThirdPersonCamera().gameObject.GetComponent<TrackedPoseDriver>().enabled = false;

                break;

            case ControlMode.SmartphonePoseCalibration:
                GetMainMenuCamera().tag = "Untagged";
                GetFirstPersonCamera().tag = "MainCamera";
                GetThirdPersonCamera().tag = "Untagged";
                CameraCache.UpdateCachedMainCamera(GetFirstPersonCamera());

                currentCamera = GetFirstPersonCamera();
                CameraOrientaionReset();

                GetMainMenuCamera().enabled = false;
                GetFirstPersonCamera().enabled = true;
                GetThirdPersonCamera().enabled = false;

                GetMainMenuCamera().gameObject.GetComponent<TrackedPoseDriver>().enabled = false;
                GetFirstPersonCamera().gameObject.GetComponent<TrackedPoseDriver>().enabled = true;
                GetThirdPersonCamera().gameObject.GetComponent<TrackedPoseDriver>().enabled = false;

                break;

            case ControlMode.SmartphonePointer:
                GetMainMenuCamera().tag = "Untagged";
                GetFirstPersonCamera().tag = "MainCamera";
                GetThirdPersonCamera().tag = "Untagged";
                CameraCache.UpdateCachedMainCamera(GetFirstPersonCamera());

                currentCamera = GetFirstPersonCamera();
                CameraOrientaionReset();

                GetMainMenuCamera().enabled = false;
                GetFirstPersonCamera().enabled = true;
                GetThirdPersonCamera().enabled = false;

                GetMainMenuCamera().gameObject.GetComponent<TrackedPoseDriver>().enabled = false;
                GetFirstPersonCamera().gameObject.GetComponent<TrackedPoseDriver>().enabled = true;
                GetThirdPersonCamera().gameObject.GetComponent<TrackedPoseDriver>().enabled = false;

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

                GetMainMenuCamera().gameObject.GetComponent<TrackedPoseDriver>().enabled = false;
                GetFirstPersonCamera().gameObject.GetComponent<TrackedPoseDriver>().enabled = true;
                GetThirdPersonCamera().gameObject.GetComponent<TrackedPoseDriver>().enabled = false;
                break;

            default:
                break;

        }
    }


    public void ActivateCamera(CameraType cameraType)
    {
        switch (cameraType)
        {
            case CameraType.MainMenu:
                EnableMainCamera(GetMainMenuCamera());
                break;

            case CameraType.FirstPerson:
                EnableMainCamera(GetFirstPersonCamera());
                break;

            case CameraType.ThirdPerson:
                EnableMainCamera(GetThirdPersonCamera());
                break;

            case CameraType.BirdsView:
                // EnableMainCamera(GetBirdsViewCamera());
                break;
        }
    }

    void EnableMainCamera(Camera camera)
    {
        currentCamera = camera;

        camera.tag = "MainCamera";
        camera.enabled = true;
        camera.gameObject.GetComponent<TrackedPoseDriver>().enabled = true;

        CameraCache.UpdateCachedMainCamera(camera);

        foreach (CameraInfo camInfo in cameraInfos)
        {
            if (camInfo.camera != currentCamera)
            {
                camInfo.camera.tag = "Untagged";
                camInfo.camera.enabled = false;
                camInfo.camera.gameObject.GetComponent<TrackedPoseDriver>().enabled = false;
            }
        }

        CameraOrientaionReset();
    }

    Camera GetMainMenuCamera() => cameraDic[CameraType.MainMenu].camera;
    Camera GetFirstPersonCamera() => cameraDic[CameraType.FirstPerson].camera;
    Camera GetThirdPersonCamera() => cameraDic[CameraType.ThirdPerson].camera;
    Camera GetBirdsViewCamera() => cameraDic[CameraType.BirdsView].camera;
}
