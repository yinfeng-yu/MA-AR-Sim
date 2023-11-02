using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandSender: MonoBehaviour
{
    public void RequestSwitchHand()
    {
        if (!HandController.IsHandFrozen) { return; }
        Command switchHandCommand = new Command(CommandType.SwitchHand);
        switchHandCommand.handedness = HandController.Instance.currentHandedness;
        TransmissionManager.Instance.SendTo(new CommandMessage(switchHandCommand), Platform.Smartphone);
    }
}
