using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagicLeap;

/// <summary>
/// In this mode the user can freely look around but cannot control the Roboy.
/// </summary>
public class HandTrackingControlMode : BaseControlMode
{

    public override void EnterControlMode(ControlModeManager a_controlModeManager)
    {
        HandControl.Instance.SetHandsInControl(true);
        HandIK.Instance.fingerTracking = true;
        HandControl.Instance.InitHandPose();
    }

    public override void UpdateControlMode(ControlModeManager a_controlModeManager)
    {
        HandControl.Instance.HandTrackingMove();
    }

    public override void ExitControlMode(ControlModeManager a_controlModeManager)
    {
        HandControl.Instance.SetHandsInControl(false);
    }
}
