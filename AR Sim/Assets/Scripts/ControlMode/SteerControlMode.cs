using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteerControlMode : BaseControlMode
{
    public override void EnterControlMode(ControlModeManager a_controlModeManager)
    {
    }

    public override void UpdateControlMode(ControlModeManager a_controlModeManager)
    {
        RoboyComponentsAccess.instance.roboyMovement.Steer(RemoteInput.GetDirection());
    }

    public override void ExitControlMode(ControlModeManager a_controlModeManager)
    {
    }
}
