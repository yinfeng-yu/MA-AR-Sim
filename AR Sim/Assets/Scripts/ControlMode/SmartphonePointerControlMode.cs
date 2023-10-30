using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartphonePointerControlMode : BaseControlMode
{
    public override void EnterControlMode(ControlModeManager a_controlModeManager)
    {
        SmartphoneController.Instance.ShowPointer();
        SmartphoneController.Instance.ShowPointerLine();
        SmartphoneController.Instance.SetPointerPosPointerMode(Handedness.Left);

        HandController.Freeze();
        HandController.Instance.InitHandPose();
    }

    public override void UpdateControlMode(ControlModeManager a_controlModeManager)
    {
        HandController.Instance.SmartphonePointerMove(HandController.Instance.currentHandedness);
    }

    public override void ExitControlMode(ControlModeManager a_controlModeManager)
    {
        SmartphoneController.Instance.HidePointer();
        SmartphoneController.Instance.HidePointerLine();
        HandController.Freeze();
    }
}
