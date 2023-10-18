using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagicLeap;

/// <summary>
/// In this mode the user can freely look around but cannot control the Roboy.
/// </summary>
public class SmartphoneControlMode : BaseControlMode
{

    public override void EnterControlMode(ControlModeManager a_controlModeManager)
    {
        SmartphoneController.Instance.ShowPointer();
        // SmartPhoneController.instance.TurnOnPointerLine();
        // HandControl.instance.SetHandsInControl(true);
        HandIK.Instance.fingerTracking = false;
        HandControl.Instance.InitHandPose();
    }

    public override void UpdateControlMode(ControlModeManager a_controlModeManager)
    {
        HandControl.Instance.SmartphoneMove(RoboyComponentsAccess.instance.roboyHands.handedness, a_controlModeManager.initOrientation, a_controlModeManager.initPosition);
    }

    public override void ExitControlMode(ControlModeManager a_controlModeManager)
    {
        SmartphoneController.Instance.ShowPointer();
        // SmartPhoneController.instance.TurnOffPointerLine();
        HandControl.Instance.SetHandsInControl(false);
    }
}
