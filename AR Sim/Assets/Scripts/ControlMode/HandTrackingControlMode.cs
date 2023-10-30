using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagicLeap;

/// <summary>
/// In this mode the user can freely look around but cannot control the Roboy.
/// </summary>
public class HandTrackingControlMode : BaseControlMode
{

    public override void EnterControlMode(ControlModeManager controlModeManager)
    {
        HandController.Unfreeze();
        HandController.Instance.InitHandPose();
    }

    public override void UpdateControlMode(ControlModeManager controlModeManager)
    {
        HandController.Instance.HandTrackingMove();
    }

    public override void ExitControlMode(ControlModeManager controlModeManager)
    {
        HandController.Freeze();
    }
}
