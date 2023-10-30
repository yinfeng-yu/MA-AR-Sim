using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteerControlMode : BaseControlMode
{
    public override void EnterControlMode(ControlModeManager controlModeManager)
    {
    }

    public override void UpdateControlMode(ControlModeManager controlModeManager)
    {
        try
        {
            Robody.Instance.movement.Steer(RemoteInput.GetDirection());
        }
        catch { }
    }

    public override void ExitControlMode(ControlModeManager controlModeManager)
    {
    }
}
