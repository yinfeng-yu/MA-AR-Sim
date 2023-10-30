using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BaseControlMode
{
    public abstract void EnterControlMode(ControlModeManager a_controlModeManager);
    public abstract void UpdateControlMode(ControlModeManager a_controlModeManager);
    public abstract void ExitControlMode(ControlModeManager a_controlModeManager);
}
