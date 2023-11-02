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

    private static bool _isHandFrozen = true;

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

    public float minPointerRange = 0.1f;
    public float maxPointerRange = 1.5f;

    bool poseInitialized = false;

    public static void Freeze()
    {
        UI.ShowToast("Hands Frozen!", 1.5f);
        _isHandFrozen = true;
    }

    public static void Unfreeze()
    {
        UI.ShowToast("Hands Unfrozen!", 1.5f);
        _isHandFrozen = false;
    }

    public static void SwitchFrozen(bool isFrozen)
    {
        UI.ShowToast(_isHandFrozen == true ? "Hands Unfrozen!" : "Hands Frozen!", 1.5f);
        _isHandFrozen = !_isHandFrozen;
    }

    public static bool IsHandFrozen => _isHandFrozen;

    public void InitHandPose()
    {
        // if (poseInitialized)
        // {
        //     return;
        // }
        // leftHandIKTarget.position = new Vector3(-0.5f, 1.6f, 0.7f);
        // rightHandIKTarget.position = new Vector3(0.5f, 1.6f, 0.7f);
    }

    public void SmartphoneMove()
    {
        if (_isHandFrozen)
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

        float robodyRotY = Robody.Instance.RobodyTransform.eulerAngles.y;

        smartphone.transform.rotation = Quaternion.Euler(Vector3.up * robodyRotY) * smartphone.transform.rotation;

        // smartphone.transform.position = SmartphoneController.Instance.calibTarget.transform.position +
        //     Mathf.Abs(SmartphoneController.Instance.calibTarget.transform.localPosition.y) * Vector3.up + Quaternion.Euler(Vector3.up * initAngleOffsetY) * (devicePosition - initPositionOffset);

        SmartphoneController.Instance.ResetPointerPos();
        smartphone.transform.position += smartphoneDistanceRatio * (Quaternion.Euler(Vector3.up * robodyRotY) * (devicePosition - initPositionOffset));


        switch (currentHandedness)
        {
            case Handedness.Left:
                var rotationLeft = smartphone.transform.rotation
                                                * Quaternion.Euler(Vector3.forward * leftHandRotationOffset.z)
                                                * Quaternion.Euler(Vector3.right * leftHandRotationOffset.x)
                                                * Quaternion.Euler(Vector3.up * leftHandRotationOffset.y);
                var positionLeft = smartphone.transform.position;

                // leftHandIKTarget.rotation = smartphone.transform.rotation 
                //                                 * Quaternion.Euler(Vector3.forward * leftHandRotationOffset.z) 
                //                                 * Quaternion.Euler(Vector3.right * leftHandRotationOffset.x)
                //                                 * Quaternion.Euler(Vector3.up * leftHandRotationOffset.y);
                // leftHandIKTarget.position = smartphone.transform.position;

                MoveIKTarget(leftHandIKTarget, positionLeft, rotationLeft, ref leftPosLerpVelocity, ref leftRotLerpVelocity);
                break;
            case Handedness.Right:
                var rotationRight = smartphone.transform.rotation
                                                * Quaternion.Euler(Vector3.forward * rightHandRotationOffset.z)
                                                * Quaternion.Euler(Vector3.right * rightHandRotationOffset.x)
                                                * Quaternion.Euler(Vector3.up * rightHandRotationOffset.y);
                var positionRight = smartphone.transform.position;

                // rightHandIKTarget.rotation = smartphone.transform.rotation
                //                                 * Quaternion.Euler(Vector3.forward * rightHandRotationOffset.z)
                //                                 * Quaternion.Euler(Vector3.right * rightHandRotationOffset.x)
                //                                 * Quaternion.Euler(Vector3.up * rightHandRotationOffset.y);
                // rightHandIKTarget.position = smartphone.transform.position;

                MoveIKTarget(rightHandIKTarget, positionRight, rotationRight, ref rightPosLerpVelocity, ref rightRotLerpVelocity);
                break;
            default:
                break;
        }

    }

    public void SmartphonePointerMove()
    {
        if (_isHandFrozen)
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

        deviceRange = Mathf.Lerp(minPointerRange, maxPointerRange, deviceRange);
        
        smartphonePointer.transform.localRotation = deviceOrientation;
        SmartphoneController.Instance.SetPointerPosPointerMode(currentHandedness);

        pointerLine.transform.localScale = new Vector3(pointerLine.transform.localScale.x, pointerLine.transform.localScale.y, deviceRange);

        switch (currentHandedness)
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
        
        if (_isHandFrozen)
        {
            return;
        }

        // Debug.Log($"screen width / height = {Screen.width} / {Screen.height}, screen pos raw = ({MediaPipeHandTracking.Instance.handRootScreenPos[0]}, {MediaPipeHandTracking.Instance.handRootScreenPos[1]}), " +
        //     $"hand screen pos = ({handScreenPosX}, {handScreenPosY}), to world = {leftHandWorldPos}");

        switch (currentHandedness)
        {
            case Handedness.Left:
                Quaternion leftLookRotation = MediaPipeHandTracking.Instance.palmForward == Vector3.zero ?
                    leftHandIKTarget.rotation : Quaternion.LookRotation(MediaPipeHandTracking.Instance.palmForward, -MediaPipeHandTracking.Instance.palmNorm);
                MoveIKTarget(leftHandIKTarget, MediaPipeHandTracking.Instance.handSkeleton.position, leftLookRotation, ref leftPosLerpVelocity, ref leftRotLerpVelocity);

                break;

            case Handedness.Right:
                Quaternion rightLookRotation = MediaPipeHandTracking.Instance.palmForward == Vector3.zero ?
                    rightHandIKTarget.rotation : Quaternion.LookRotation(MediaPipeHandTracking.Instance.palmForward, -MediaPipeHandTracking.Instance.palmNorm);
                MoveIKTarget(rightHandIKTarget, MediaPipeHandTracking.Instance.handSkeleton.position, rightLookRotation, ref rightPosLerpVelocity, ref rightRotLerpVelocity);
                
                break;
            default:
                break;
        }

        // if (trackLeftHand)
        // {
        //     try
        //     {
        //         MediaPipeHandTracking.Instance.leftHandSkeleton.position = leftHandWorldPos;
        //         MediaPipeHandTracking.Instance.leftHandSkeleton.LookAt(MediaPipeHandTracking.Instance.leftHandSkeleton.position + Camera.main.transform.forward);
        // 
        //     }
        //     catch { }
        //     MoveIKTarget(leftHandIKTarget, MediaPipeHandTracking.Instance.leftHandSkeleton.position, leftLookRotation, ref leftPosLerpVelocity, ref leftRotLerpVelocity);
        // }
        // 
        // if (trackRightHand)
        // {
        //     try
        //     {
        //         MediaPipeHandTracking.Instance.rightHandSkeleton.position = rightHandWorldPos;
        //         MediaPipeHandTracking.Instance.rightHandSkeleton.LookAt(MediaPipeHandTracking.Instance.rightHandSkeleton.position + Camera.main.transform.forward);
        // 
        //     }
        //     catch { }
        //     MoveIKTarget(rightHandIKTarget, MediaPipeHandTracking.Instance.rightHandSkeleton.position, rightLookRotation, ref rightPosLerpVelocity, ref rightRotLerpVelocity);
        // }
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
        if (_isHandFrozen)
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
