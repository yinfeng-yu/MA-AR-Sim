using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class NetworkMessageHandler
{
    public static void ProcessMessage(string rawMessage, NetworkMessage currentMessage, NetworkTransmitter networkTransmitter)
    {
        // var peers = transmissionManager.GetPeers();
        switch ((NetworkMessageType)currentMessage.ty)
        {
            case NetworkMessageType.StreamMessage:
                // StreamMessage receivedStreamMessage = NetworkUtilities.UnpackMessage<StreamMessage>(rawMessage);
                // StreamDataHeader receivedHeader = JsonUtility.FromJson<StreamDataHeader>(receivedStreamMessage.d);
                // EventManager.Instance.InvokeStreamUpdated(receivedHeader, receivedStreamMessage.v);
                break;

            case NetworkMessageType.AwakeMessage:
                //if this peer hasn't been gone long then fire a recap:
                if (networkTransmitter.peers.ContainsKey(currentMessage.a))
                {
                    // OnPeerFound?.Invoke(currentMessage.f, long.Parse(currentMessage.d));
                    networkTransmitter.peers[currentMessage.a].age = Time.realtimeSinceStartup;
                }
                break;

            case NetworkMessageType.HeartbeatMessage:
                //new peer:
                if (!networkTransmitter.peers.ContainsKey(currentMessage.a))
                {
                    networkTransmitter.peers.Add(currentMessage.a, new Peer(currentMessage.a, currentMessage.f, 0));

                }
                //catalog heartbeat time:
                networkTransmitter.peers[currentMessage.a].age = Time.realtimeSinceStartup;
                break;

            case NetworkMessageType.NotificationMessage:
                // Debug.Log($"Notification received! Message: {rawMessage}");
                // Notification notification = NetworkUtilities.UnpackMessage<Notification>(currentMessage.d);
                // NotificationManager.instance.Notify(notification);
                break;

            case NetworkMessageType.Vector3Message:
                // Debug.Log($"Vector3 received! Message: {rawMessage}");
                Vector3Message receivedVector3Message = NetworkUtilities.UnpackMessage<Vector3Message>(rawMessage);
                GlobalVariableManager.Instance.GlobalVector3s[receivedVector3Message.l] = receivedVector3Message.v;
                break;

            case NetworkMessageType.QuaternionMessage:
                
                QuaternionMessage receivedQuaternionMessage = NetworkUtilities.UnpackMessage<QuaternionMessage>(rawMessage);
                // Debug.Log($"Quaternion Message Received, label = {receivedQuaternionMessage.l}");
                GlobalVariableManager.Instance.GlobalQuaternions[receivedQuaternionMessage.l] = receivedQuaternionMessage.q;
                break;

            case NetworkMessageType.FloatMessage:
                // Debug.Log("Float Message Received");
                FloatMessage receivedFloatMessage = NetworkUtilities.UnpackMessage<FloatMessage>(rawMessage);
                GlobalVariableManager.Instance.GlobalFloats[receivedFloatMessage.l] = receivedFloatMessage.fl;
                break;

            case NetworkMessageType.CommandMessage:
                CommandMessage receivedCommandMessage = NetworkUtilities.UnpackMessage<CommandMessage>(rawMessage);
                HandleCommand(receivedCommandMessage.co);
                break;

            case NetworkMessageType.OperationMessage:
                OperationMessage receivedBaseControlMessage = NetworkUtilities.UnpackMessage<OperationMessage>(rawMessage);
                HandleOperation(receivedBaseControlMessage.o);
                break;

        }
    }

    static void HandleOperation(Operation operation)
    {
        
        switch (operation.type)
        {
            case OperationType.Confirm:
                if (operation.confirm)
                {
                    RemoteInput.SetConfirm();
                }
                break;

            case OperationType.ChangeStreamView:
                break;

            case OperationType.Steer:
                RemoteInput.SetDirection(operation.direction);
                break;

            case OperationType.SwitchControlMode:
                ControlModeManager.Instance.SwitchToMainMenu();
                break;

            case OperationType.Freeze:
                HandController.SwitchFrozen(operation.isFrozen);
                break;
         
        }
    }

    static void HandleCommand(Command command)
    {
        switch (command.type)
        {
            case CommandType.Grab:
                Debug.Log("Grab Command Received.");

                if (command.handedness == Handedness.Left)
                {
                    // if (ControlModeManager.Instance.currentControlMode == ControlMode.SmartphonePointer)
                    // {
                    //     if (!command.isGrab) FingerController.Instance.isLeftGrab = !FingerController.Instance.isLeftGrab;
                    // }
                    // else
                    // {
                    //     FingerController.Instance.isLeftGrab = command.isGrab;
                    // }

                    if (!command.isGrab) FingerController.Instance.isLeftGrab = !FingerController.Instance.isLeftGrab;
                }
                else if (command.handedness == Handedness.Right)
                {
                    // if (ControlModeManager.Instance.currentControlMode == ControlMode.SmartphonePointer)
                    // {
                    //     if (!command.isGrab) FingerController.Instance.isRightGrab = !FingerController.Instance.isRightGrab;
                    // }
                    // else
                    // {
                    //     FingerController.Instance.isRightGrab = command.isGrab;
                    // }

                    if (!command.isGrab) FingerController.Instance.isRightGrab = !FingerController.Instance.isRightGrab;
                }

                break;

            case CommandType.Displace:
                Debug.Log($"Target Location: {command.targetLocation}");
                NavigationManager.instance.SetDestination(command.targetLocation);
                break;

            case CommandType.Patrol:
                Debug.Log("Patrol Command Received.");
                if (command.waypoints.Length == 0)
                {
                    NavigationManager.instance.StopPatrol();
                }
                else
                {
                    NavigationManager.instance.StartPatrol(command.waypoints);
                }
                break;

            case CommandType.SwitchHand:
                HandController.Instance.currentHandedness = command.handedness;
                if (ControlModeManager.Instance.currentControlMode == ControlMode.SmartphonePointer)
                {
                    SmartphoneController.Instance.SetPointerPosPointerMode(command.handedness);
                }
                break;

            default:
                break;
        }
    }

}