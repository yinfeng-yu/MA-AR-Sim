using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagicLeap;
using Mediapipe.Unity.Tutorial;

/// <summary>
/// In this mode the user can freely look around but cannot control the Roboy.
/// </summary>
public class HandTrackingControlMode : BaseControlMode
{

    public override void EnterControlMode(ControlModeManager controlModeManager)
    {
        HandController.Freeze();
        HandController.Instance.InitHandPose();
        MediaPipeHandTracking.Instance.handSkeleton.gameObject.SetActive(true);
    }

    public override void UpdateControlMode(ControlModeManager controlModeManager)
    {
        HandController.Instance.HandTrackingMove();
        try
        {
            Robody.Instance.movement.Steer(RemoteInput.GetDirection());
        }
        catch { }
    }

    public override void ExitControlMode(ControlModeManager controlModeManager)
    {
        HandController.Freeze();
        MediaPipeHandTracking.Instance.handSkeleton.gameObject.SetActive(false);
    }
}
