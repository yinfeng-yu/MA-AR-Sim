using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagicLeap;

/// <summary>
/// In this mode the user can freely look around but cannot control the Roboy.
/// </summary>
public class SmartphoneControlMode : BaseControlMode
{

    public override void EnterControlMode(ControlModeManager controlModeManager)
    {
        HandController.Freeze();
        SmartphoneController.Instance.ShowPointer();
        HandController.Instance.InitHandPose();
    }

    public override void UpdateControlMode(ControlModeManager controlModeManager)
    {
        HandController.Instance.SmartphoneMove(HandController.Instance.currentHandedness, SmartphoneController.Instance.initOrientation, SmartphoneController.Instance.initPosition);
    }

    public override void ExitControlMode(ControlModeManager controlModeManager)
    {
        SmartphoneController.Instance.HidePointer();
        HandController.Freeze();
    }
}
