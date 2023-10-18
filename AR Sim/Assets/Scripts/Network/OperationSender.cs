using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationSender : Singleton<OperationSender>
{
    public void SendSwitchControlMode(ControlMode controlMode)
    {
        Operation operation = new Operation();
        operation.type = OperationType.SwitchControlMode;
        operation.controlMode = controlMode;

        TransmissionManager.Instance.SendTo(new OperationMessage(operation), Platform.Smartphone);
    }
}
