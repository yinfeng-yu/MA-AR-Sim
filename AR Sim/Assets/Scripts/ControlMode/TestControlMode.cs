using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestControlMode : BaseControlMode
{
    public override void EnterControlMode(ControlModeManager controlModeManager)
    {
        RobodyHands.DisableIK();
    }

    public override void UpdateControlMode(ControlModeManager controlModeManager)
    {
        
    }

    public override void ExitControlMode(ControlModeManager controlModeManager)
    {
        RobodyHands.EnableIK();
    }
}
