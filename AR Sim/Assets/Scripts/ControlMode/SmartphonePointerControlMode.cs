using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartphonePointerControlMode : BaseControlMode
{
    public override void EnterControlMode(ControlModeManager a_controlModeManager)
    {
        SmartphoneController.Instance.ShowPointer();
        SmartphoneController.Instance.ShowPointerLine();
        // HandControl.instance.SetHandsInControl(true);
        HandIK.Instance.fingerTracking = false;
        HandControl.Instance.InitHandPose();
    }

    public override void UpdateControlMode(ControlModeManager a_controlModeManager)
    {
        HandControl.Instance.SmartphonePointerMove(RoboyComponentsAccess.instance.roboyHands.handedness);
    }

    public override void ExitControlMode(ControlModeManager a_controlModeManager)
    {
        SmartphoneController.Instance.HidePointer();
        SmartphoneController.Instance.HidePointerLine();
        HandControl.Instance.SetHandsInControl(false);
    }
}
