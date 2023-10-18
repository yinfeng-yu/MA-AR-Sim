using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartphonePoseCalibrationControlMode : BaseControlMode
{
    public Quaternion initOrientation = Quaternion.identity;
    public Vector3 initPosition = Vector3.zero;
    
    public bool isCalibrating = true;

    public override void EnterControlMode(ControlModeManager a_controlModeManager)
    {
        HandControl.Instance.SetHandsInControl(false);
        // initOrientation = Camera.main.transform.rotation;
        SmartphoneController.Instance.ShowCalibTarget();
        SmartphoneController.Instance.ShowPointer();
    }

    public override void UpdateControlMode(ControlModeManager a_controlModeManager)
    {
        if (isCalibrating)
        {
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
                Debug.Log("start calibrating");
            }
            else
            {
                Debug.Log("stop calibrating");

                Vector3 projectionA = Vector3.ProjectOnPlane(Vector3.forward, Vector3.up);
                Vector3 projectionB = Vector3.ProjectOnPlane(initOrientation * Vector3.forward, Vector3.up);
                float angle = Vector3.Angle(projectionA, projectionB);
                HandControl.Instance.initAngleOffsetY = angle;
                HandControl.Instance.initPositionOffset = initPosition - HandControl.Instance.calibTarget.localPosition;

                a_controlModeManager.initOrientation = Quaternion.Inverse(a_controlModeManager.calibTarget.rotation) * initOrientation;
                a_controlModeManager.initPosition = initPosition;
            }
        }


    }

    public override void ExitControlMode(ControlModeManager a_controlModeManager)
    {
        SmartphoneController.Instance.HideCalibTarget();
        SmartphoneController.Instance.HidePointer();
    }
}
