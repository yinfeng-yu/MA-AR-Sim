using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OperationType
{
    Confirm,
    ChangeStreamView,
    Steer,
    SwitchControlMode,
    Freeze,
}

[Serializable]
public class Operation
{
    public OperationType type;

    public bool confirm;
    public int streamView;
    public Direction direction;

    public ControlMode controlMode;
    public bool isFrozen;
}
