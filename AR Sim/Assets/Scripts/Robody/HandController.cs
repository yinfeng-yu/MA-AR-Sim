using Mediapipe.Unity.Tutorial;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
// using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Handedness
{
    Left,
    Right,
    Both,
}

[Serializable]
public struct HandRotationOffset
{
    public float x; public float y; public float z;
}

public class HandController : Singleton<HandController>
{
    [Header("Hand Settings")]
    public Handedness currentHandedness = Handedness.Left;

    [SerializeField] HandRotationOffset leftHandRotationOffset = new HandRotationOffset{ x = 90, y = 90, z = 90 };
    [SerializeField] HandRotationOffset rightHandRotationOffset = new HandRotationOffset{ x = -90, y = -90, z = -90 };

    public Transform leftHandIKTarget;
    public Transform rightHandIKTarget;

    // public Transform HandTrackingRoot;
    // public HandTracking HandTracking;
    public float handDistanceRatio = 0.1f;

    private static bool ishandFrozen = true;

    // public Transform leftHandPalm;
    public float initAngleOffsetY = 0f;
    public Vector3 initPositionOffset = Vector3.zero;

    public Transform handPos;

    Vector3 leftPosLerpVelocity;
    Vector3 rightPosLerpVelocity;

    [Header("Hand Lerp Settings")]
    Quaternion leftRotLerpVelocity;
    Quaternion rightRotLerpVelocity;

    public float lerpDuration = 0.3f;
    public float handPosChangeThreshold = 3f;

    public float smartphoneDistanceRatio = 2;

    public bool trackLeftHand = true;
    public bool trackRightHand = false;

    bool poseInitialized = false;

    public static void Freeze()
    {
        UI.ShowToast("Hands Frozen!", 1.5f);
        ishandFrozen = true;
    }

    public static void Unfreeze()
    {
        UI.ShowToast("Hands Unfrozen!", 1.5f);
        ishandFrozen = false;
    }

    public static void SwitchFrozen(bool isFrozen)
    {
        UI.ShowToast(ishandFrozen == true ? "Hands Unfrozen!" : "Hands Frozen!", 1.5f);
        ishandFrozen = !ishandFrozen;
    }

    public void InitHandPose()
    {
        // if (poseInitialized)
        // {
        //     return;
        // }
        // leftHandIKTarget.position = new Vector3(-0.5f, 1.6f, 0.7f);
        // rightHandIKTarget.position = new Vector3(0.5f, 1.6f, 0.7f);
    }

    public void SmartphoneMove(Handedness handedness, Quaternion initOrientation, Vector3 initPosition)
    {
        if (ishandFrozen)
        {
            return;
        }

        Quaternion deviceOrientation;
        Vector3 devicePosition;

        var smartphone = SmartphoneController.Instance.smartphonePointer;

        GlobalVariableManager.Instance.GlobalQuaternions.TryGetValue("deviceOrientation", out deviceOrientation);
        GlobalVariableManager.Instance.GlobalVector3s.TryGetValue("devicePosition", out devicePosition);

        // Debug.Log($"device orientation = {deviceOrientation}");
        // Debug.Log($"original rot = {deviceOrientation}, init rot = {initOrientation}, final rot = {deviceOrientation * Quaternion.Inverse(initOrientation)}");
        smartphone.transform.rotation = deviceOrientation; // Quaternion.Inverse(initOrientation);
        smartphone.transform.rotation = Quaternion.Euler(Vector3.up * initAngleOffsetY) * smartphone.transform.rotation;

        // smartphone.transform.position = SmartphoneController.Instance.calibTarget.transform.position +
        //     Mathf.Abs(SmartphoneController.Instance.calibTarget.transform.localPosition.y) * Vector3.up + Quaternion.Euler(Vector3.up * initAngleOffsetY) * (devicePosition - initPositionOffset);

        SmartphoneController.Instance.ResetPointerPos();
        smartphone.transform.position += smartphoneDistanceRatio * (Quaternion.Euler(Vector3.up * initAngleOffsetY) * (devicePosition - initPositionOffset));


        switch (handedness)
        {
            case Handedness.Left:
                leftHandIKTarget.rotation = smartphone.transform.rotation 
                                                * Quaternion.Euler(Vector3.forward * leftHandRotationOffset.z) 
                                                * Quaternion.Euler(Vector3.right * leftHandRotationOffset.x)
                                                * Quaternion.Euler(Vector3.up * leftHandRotationOffset.y);
                leftHandIKTarget.position = smartphone.transform.position; // * 2f;
                break;
            case Handedness.Right:
                rightHandIKTarget.rotation = smartphone.transform.rotation; // * Quaternion.Euler(Vector3.forward * rightHandRotationZOffset);
                rightHandIKTarget.localPosition = smartphone.transform.localPosition; // * 2f;
                break;
            default:
                break;
        }

    }

    public void SmartphonePointerMove(Handedness handedness)
    {
        if (ishandFrozen)
        {
            return;
        }

        Quaternion deviceOrientation;
        // Vector3 devicePosition;
        float deviceRange;

        var smartphonePointer = SmartphoneController.Instance.smartphonePointer;
        var pointerLine = SmartphoneController.Instance.pointerLine;

        GlobalVariableManager.Instance.GlobalQuaternions.TryGetValue("deviceOrientation", out deviceOrientation);
        GlobalVariableManager.Instance.GlobalFloats.TryGetValue("deviceRange", out deviceRange);
        
        smartphonePointer.transform.localRotation = deviceOrientation;
        SmartphoneController.Instance.SetPointerPosPointerMode(currentHandedness);

        pointerLine.transform.localScale = new Vector3(pointerLine.transform.localScale.x, pointerLine.transform.localScale.y, deviceRange);

        switch (handedness)
        {
            case Handedness.Left:
                var leftTargetRotation = smartphonePointer.transform.rotation * Quaternion.Euler(Vector3.forward * leftHandRotationOffset.z)
                                                * Quaternion.Euler(Vector3.right * leftHandRotationOffset.x)
                                                * Quaternion.Euler(Vector3.up * leftHandRotationOffset.y);
                var leftTargetPosition = smartphonePointer.transform.position + smartphonePointer.transform.up * deviceRange;

                MoveIKTarget(leftHandIKTarget, leftTargetPosition, leftTargetRotation, ref leftPosLerpVelocity, ref leftRotLerpVelocity);
                break;

            case Handedness.Right:
                var rightTargetRotation = smartphonePointer.transform.rotation * Quaternion.Euler(Vector3.forward * rightHandRotationOffset.z)
                                                * Quaternion.Euler(Vector3.right * rightHandRotationOffset.x)
                                                * Quaternion.Euler(Vector3.up * rightHandRotationOffset.y);
                var rightTargetPosition = smartphonePointer.transform.position + smartphonePointer.transform.up * deviceRange;

                MoveIKTarget(rightHandIKTarget, rightTargetPosition, rightTargetRotation, ref rightPosLerpVelocity, ref rightRotLerpVelocity);
                break;
            default:
                break;
        }
        
    }

    public void HandTrackingMove()
    {
        
        if (ishandFrozen)
        {
            return;
        }

        float videoScreenWidth = 640;
        float videoScreenHeight = 480;
        float widthSVRatio = Screen.width / videoScreenWidth;
        float heightSVRatio = Screen.height / videoScreenHeight;

        float leftHandScreenPosX = widthSVRatio * (videoScreenWidth / 2 + MediaPipeHandTracking.Instance.leftHandRootScreenPos[0]);
        float leftHandScreenPosY = heightSVRatio * (videoScreenHeight / 2 + MediaPipeHandTracking.Instance.leftHandRootScreenPos[1]);

        float rightHandScreenPosX = widthSVRatio * (videoScreenWidth / 2 + MediaPipeHandTracking.Instance.rightHandRootScreenPos[0]);
        float rightHandScreenPosY = heightSVRatio * (videoScreenHeight / 2 + MediaPipeHandTracking.Instance.rightHandRootScreenPos[1]);

        Vector3 leftHandWorldPos = Camera.main.ScreenToWorldPoint(
            new Vector3(leftHandScreenPosX, leftHandScreenPosY, MediaPipeHandTracking.Instance.leftHandDistance * handDistanceRatio)
            ) + MediaPipeHandTracking.Instance.leftHandRootTranslation;

        Vector3 rightHandWorldPos = Camera.main.ScreenToWorldPoint(
            new Vector3(rightHandScreenPosX, rightHandScreenPosY, MediaPipeHandTracking.Instance.rightHandDistance * handDistanceRatio)
            ) + MediaPipeHandTracking.Instance.rightHandRootTranslation;

        // Debug.Log($"screen width / height = {Screen.width} / {Screen.height}, screen pos raw = ({MediaPipeHandTracking.Instance.handRootScreenPos[0]}, {MediaPipeHandTracking.Instance.handRootScreenPos[1]}), " +
        //     $"hand screen pos = ({handScreenPosX}, {handScreenPosY}), to world = {leftHandWorldPos}");

        Quaternion leftLookRotation = MediaPipeHandTracking.Instance.leftPalmForward == Vector3.zero ?
            leftHandIKTarget.rotation : Quaternion.LookRotation(MediaPipeHandTracking.Instance.leftPalmForward, MediaPipeHandTracking.Instance.leftPalmNorm);

        Quaternion rightLookRotation = MediaPipeHandTracking.Instance.rightPalmForward == Vector3.zero ?
            rightHandIKTarget.rotation : Quaternion.LookRotation(MediaPipeHandTracking.Instance.rightPalmForward, -MediaPipeHandTracking.Instance.rightPalmNorm);

        if (trackLeftHand)
        {
            try
            {
                MediaPipeHandTracking.Instance.leftHandSkeleton.position = leftHandWorldPos;
            }
            catch (Exception e) { }
            MoveIKTarget(leftHandIKTarget, MediaPipeHandTracking.Instance.leftHandSkeleton.position, leftLookRotation, ref leftPosLerpVelocity, ref leftRotLerpVelocity);
        }

        if (trackRightHand)
        {
            try
            {
                MediaPipeHandTracking.Instance.rightHandSkeleton.position = rightHandWorldPos;
            }
            catch (Exception e) { }
            MoveIKTarget(rightHandIKTarget, MediaPipeHandTracking.Instance.rightHandSkeleton.position, rightLookRotation, ref rightPosLerpVelocity, ref rightRotLerpVelocity);
        }
    }

    Quaternion SmoothDamp(Quaternion rot, Quaternion target, ref Quaternion deriv, float time)
    {
        if (Time.deltaTime < Mathf.Epsilon) return rot;
        // account for double-cover
        var Dot = Quaternion.Dot(rot, target);
        var Multi = Dot > 0f ? 1f : -1f;
        target.x *= Multi;
        target.y *= Multi;
        target.z *= Multi;
        target.w *= Multi;
        // smooth damp (nlerp approx)
        var Result = new Vector4(
            Mathf.SmoothDamp(rot.x, target.x, ref deriv.x, time),
            Mathf.SmoothDamp(rot.y, target.y, ref deriv.y, time),
            Mathf.SmoothDamp(rot.z, target.z, ref deriv.z, time),
            Mathf.SmoothDamp(rot.w, target.w, ref deriv.w, time)
        ).normalized;

        // ensure deriv is tangent
        var derivError = Vector4.Project(new Vector4(deriv.x, deriv.y, deriv.z, deriv.w), Result);
        deriv.x -= derivError.x;
        deriv.y -= derivError.y;
        deriv.z -= derivError.z;
        deriv.w -= derivError.w;

        return new Quaternion(Result.x, Result.y, Result.z, Result.w);
    }

    /// <summary>
    /// Used to smoothly move the IK target
    /// </summary>
    /// <param name="ikTarget"></param>
    /// <param name="targetPosition"></param>
    /// <param name="targetRotation"></param>
    /// <param name="posLerpVelocity"></param>
    /// <param name="rotLerpVelocity"></param>
    void MoveIKTarget(Transform ikTarget, Vector3 targetPosition, Quaternion targetRotation, ref Vector3 posLerpVelocity, ref Quaternion rotLerpVelocity)
    {
        // if (Vector3.Distance(ikTarget.position, targetPosition) >= handPosChangeThreshold)
        // {
        //     return;
        // }

        var lerpedPos = Vector3.SmoothDamp(ikTarget.position, targetPosition, ref posLerpVelocity, lerpDuration);
        var lerpedRot = SmoothDamp(ikTarget.rotation, targetRotation, ref rotLerpVelocity, lerpDuration);

        ikTarget.position = lerpedPos;
        ikTarget.rotation = lerpedRot;
    }

    public void ControllerMove()
    {
        if (ishandFrozen)
        {
            return;
        }

        // Controller
        // foreach (IMixedRealityController controller in CoreServices.InputSystem.DetectedControllers)
        // {
        //     foreach (MixedRealityInteractionMapping interactionMapping in controller.Interactions)
        //     {
        //         // 6DOF controllers support the "SpatialPointer" type (pointing direction)
        //         // or "GripPointer" type (direction of the 6DOF controller)
        //         if (interactionMapping.InputType == DeviceInputType.SpatialPointer)
        //         {
        //             // Debug.Log("Spatial pointer PositionData: " + interactionMapping.PositionData);
        //             // Debug.Log("Spatial pointer RotationData: " + interactionMapping.RotationData);
        //             if (controller.ControllerHandedness == Microsoft.MixedReality.Toolkit.Utilities.Handedness.Left)
        //             {
        //                 leftHandIKTarget.position = interactionMapping.PositionData;
        //                 leftHandIKTarget.rotation = interactionMapping.RotationData * Quaternion.Euler(Vector3.forward * leftHandRotationOffset.z);
        //             }
        // 
        //             else if (controller.ControllerHandedness == Microsoft.MixedReality.Toolkit.Utilities.Handedness.Right)
        //             {
        //                 rightHandIKTarget.position = interactionMapping.PositionData;
        //                 rightHandIKTarget.rotation = interactionMapping.RotationData * Quaternion.Euler(Vector3.forward * rightHandRotationOffset.z);
        //             }
        //         }
        // 
        //         // if (interactionMapping.InputType == DeviceInputType.Trigger)
        //         // {
        //         //     Debug.Log(interactionMapping.AxisCodeX);
        //         //     Debug.Log(interactionMapping.AxisCodeY);
        //         //     Debug.Log(interactionMapping.Changed);
        //         // }
        // 
        //         // if (interactionMapping.InputType == DeviceInputType.SpatialGrip)
        //         // {
        //         //     Debug.Log("Spatial grip PositionData: " + interactionMapping.PositionData);
        //         //     Debug.Log("Spatial grip RotationData: " + interactionMapping.RotationData);
        //         // }
        //     }
        // }
    }

}
