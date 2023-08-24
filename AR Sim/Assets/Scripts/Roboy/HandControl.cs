using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandControl : MonoBehaviour
{
    public static HandControl instance;

    private void Awake()
    {
        if (instance != this)
        {
            instance = this;
        }
    }

    public Handedness currentHandedness = Handedness.Left;

    [SerializeField] float leftHandRotationZOffset = 90;
    [SerializeField] float rightHandRotationZOffset = -90;

    [SerializeField] protected Transform leftHandIKTarget;
    [SerializeField] protected Transform rightHandIKTarget;


    private bool _handTrackingUpdate = false;
    public bool handTrackingUpdate
    {
        get => _handTrackingUpdate;
        set => _handTrackingUpdate = value;
    }


    public void SmartphoneMove()
    {
        Quaternion deviceOrientation;
        float deviceRange;

        var smartphonePointer = SmartPhoneController.instance.smartphonePointer;
        var pointerLine = SmartPhoneController.instance.pointerLine;

        TransmissionManager.instance.globalQuaternions.TryGetValue("deviceOrientation", out deviceOrientation);
        TransmissionManager.instance.globalFloats.TryGetValue("deviceRange", out deviceRange);
        
        smartphonePointer.rotation = deviceOrientation;
        smartphonePointer.position = SmartPhoneController.instance.smartphonePivot.position + 0.5f * (currentHandedness == Handedness.Right ? Vector3.right : Vector3.left);

        pointerLine.rotation = deviceOrientation;
        pointerLine.localScale = new Vector3(pointerLine.localScale.x, pointerLine.localScale.y, deviceRange);

        leftHandIKTarget.rotation = smartphonePointer.rotation * Quaternion.Euler(Vector3.forward * leftHandRotationZOffset);
        leftHandIKTarget.position = smartphonePointer.position + smartphonePointer.forward * deviceRange;
    }

    public void HandTrackingMove()
    {
        // Controller
        foreach (IMixedRealityController controller in CoreServices.InputSystem.DetectedControllers)
        {
            var hand = controller as IMixedRealityHand;
            if (hand != null)
            {
                if (hand.TryGetJoint(TrackedHandJoint.Palm, out MixedRealityPose jointPose))
                {
                    // Debug.Log(jointPose.Position);
                    if (hand.ControllerHandedness == Handedness.Left)
                    {
                        leftHandIKTarget.position = RoboyComponentsAccess.instance.roboyHands.gameObject.transform.position + jointPose.Position; // transform.position + 0.5f * jointPose.Position;
                        leftHandIKTarget.rotation = jointPose.Rotation * Quaternion.Euler(Vector3.forward * leftHandRotationZOffset);
                    }
                    else if (hand.ControllerHandedness == Handedness.Right)
                    {
                        rightHandIKTarget.position = RoboyComponentsAccess.instance.roboyHands.gameObject.transform.position + jointPose.Position; // transform.position + 0.5f * jointPose.Position;
                        rightHandIKTarget.rotation = jointPose.Rotation * Quaternion.Euler(Vector3.forward * rightHandRotationZOffset);
                    }
                }
            }
        }
    }

    public void ControllerMove()
    {
        // Controller
        foreach (IMixedRealityController controller in CoreServices.InputSystem.DetectedControllers)
        {
            foreach (MixedRealityInteractionMapping interactionMapping in controller.Interactions)
            {
                // 6DOF controllers support the "SpatialPointer" type (pointing direction)
                // or "GripPointer" type (direction of the 6DOF controller)
                if (interactionMapping.InputType == DeviceInputType.SpatialPointer)
                {
                    // Debug.Log("Spatial pointer PositionData: " + interactionMapping.PositionData);
                    // Debug.Log("Spatial pointer RotationData: " + interactionMapping.RotationData);
                    if (controller.ControllerHandedness == Handedness.Left)
                    {
                        leftHandIKTarget.position = interactionMapping.PositionData;
                        leftHandIKTarget.rotation = interactionMapping.RotationData * Quaternion.Euler(Vector3.forward * leftHandRotationZOffset);
                    }

                    else if (controller.ControllerHandedness == Handedness.Right)
                    {
                        rightHandIKTarget.position = interactionMapping.PositionData;
                        rightHandIKTarget.rotation = interactionMapping.RotationData * Quaternion.Euler(Vector3.forward * rightHandRotationZOffset);
                    }
                }

                // if (interactionMapping.InputType == DeviceInputType.Trigger)
                // {
                //     Debug.Log(interactionMapping.AxisCodeX);
                //     Debug.Log(interactionMapping.AxisCodeY);
                //     Debug.Log(interactionMapping.Changed);
                // }

                // if (interactionMapping.InputType == DeviceInputType.SpatialGrip)
                // {
                //     Debug.Log("Spatial grip PositionData: " + interactionMapping.PositionData);
                //     Debug.Log("Spatial grip RotationData: " + interactionMapping.RotationData);
                // }
            }
        }
    }

}
