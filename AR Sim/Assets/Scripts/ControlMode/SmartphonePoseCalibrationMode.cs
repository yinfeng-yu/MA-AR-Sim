using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartphonePoseCalibrationControlMode : BaseControlMode
{
    public Quaternion initOrientation = Quaternion.identity;
    public Vector3 initPosition = Vector3.zero;
    
    public bool isCalibrating = true;

    public override void EnterControlMode(ControlModeManager controlModeManager)
    {
        HandController.Freeze();
        SmartphoneController.Instance.ShowCalibTarget();
        SmartphoneController.Instance.ShowPointer();
    }

    public override void UpdateControlMode(ControlModeManager controlModeManager)
    {
        if (isCalibrating)
        {
            // Debug.Log("Calibrating...");
            Quaternion deviceOrientation;
            Vector3 devicePosition;

            GlobalVariableManager.Instance.GlobalQuaternions.TryGetValue("deviceOrientation", out deviceOrientation);
            GlobalVariableManager.Instance.GlobalVector3s.TryGetValue("devicePosition", out devicePosition);

            SmartphoneController.Instance.SetPointerPosCalibMode();
            SmartphoneController.Instance.smartphonePointer.transform.rotation = initOrientation * deviceOrientation;

            // initOrientation = Quaternion.Lerp(deviceOrientation, initOrientation, 0.5f);
            initOrientation = deviceOrientation;
            initPosition = devicePosition;
        }

        if (RemoteInput.GetConfirm())
        {
            RemoteInput.ConsumeConfirm();
            isCalibrating = !isCalibrating;
            if (isCalibrating)
            {
                Debug.Log("Start calibrating");
            }
            else
            {
                Debug.Log("Stop calibrating");

                Vector3 projectionA = Vector3.ProjectOnPlane(Vector3.forward, Vector3.up);
                Vector3 projectionB = Vector3.ProjectOnPlane(initOrientation * Vector3.forward, Vector3.up);
                float angle = Vector3.Angle(projectionA, projectionB);
                HandController.Instance.initAngleOffsetY = angle;
                HandController.Instance.initPositionOffset = initPosition - SmartphoneController.Instance.calibTarget.transform.localPosition;

                SmartphoneController.Instance.initOrientation = Quaternion.Inverse(SmartphoneController.Instance.calibTarget.transform.rotation) * initOrientation;
                SmartphoneController.Instance.initPosition = initPosition;
            }
        }


    }

    public override void ExitControlMode(ControlModeManager controlModeManager)
    {
        SmartphoneController.Instance.HideCalibTarget();
        SmartphoneController.Instance.HidePointer();
        HandController.Freeze();
    }
}
