using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NetworkMessageHandler : MonoBehaviour
{
    public void ProcessMessage(string rawMessage, NetworkMessage currentMessage, NetworkTransmitter networkTransmitter)
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

    void HandleOperation(Operation operation)
    {
        
        switch (operation.type)
        {
            case OperationType.Confirm:
                if (operation.confirm) RemoteInput.SetConfirm();
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
                HandControl.Instance.SetHandsInControl(!operation.isFrozen);
                break;
         
        }
    }

    void HandleCommand(Command command)
    {
        switch (command.type)
        {
            case CommandType.Grab:
                Debug.Log("Grab Command Received.");

                if (command.handedness == Handedness.Left)
                {
                    if (ControlModeManager.Instance.currentControlMode == ControlMode.SmartphonePointer)
                    {
                        if (!command.isGrab) HandIK.Instance.isLeftGrab = !HandIK.Instance.isLeftGrab;
                    }
                    else
                    {
                        HandIK.Instance.isLeftGrab = command.isGrab;
                    }
                    
                }
                else if (command.handedness == Handedness.Right)
                {
                    if (ControlModeManager.Instance.currentControlMode == ControlMode.SmartphonePointer)
                    {
                        if (!command.isGrab) HandIK.Instance.isRightGrab = !HandIK.Instance.isRightGrab;
                    }
                    else
                    {
                        HandIK.Instance.isRightGrab = command.isGrab;
                    }
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
                RoboyComponentsAccess.instance.roboyHands.handedness = command.handedness;
                HandControl.Instance.currentHandedness = command.handedness;
                break;

            default:
                break;
        }
    }

}




// public class NetworkMessageHandler : MonoBehaviour
// {
//     // Streaming
//     public Action<StreamDataHeader, byte[]> streamUpdated;
// 
//     public void ProcessRawMessage(string rawMessage)
//     {
//         TransmissionManager transmissionManager = GetComponent<TransmissionManager>();
//         // get message:
//         NetworkMessage currentMessage = JsonUtility.FromJson<NetworkMessage>(rawMessage);
// 
//         // debug:
//         if (transmissionManager.debugIncoming)
//         {
//             Debug.Log($"Received {rawMessage} from {currentMessage.f}");
//         }
// 
//         var peers = transmissionManager.GetPeers();
// 
//         switch ((NetworkMessageType)currentMessage.ty)
//         {
//             case NetworkMessageType.StreamMessage:
//                 StreamMessage receivedStreamMessage = NetworkUtilities.UnpackMessage<StreamMessage>(rawMessage);
//                 StreamDataHeader receivedHeader = JsonUtility.FromJson<StreamDataHeader>(receivedStreamMessage.d);
//                 streamUpdated?.Invoke(receivedHeader, receivedStreamMessage.v);
//                 break;
// 
//             case NetworkMessageType.AwakeMessage:
//                 //if this peer hasn't been gone long then fire a recap:
//                 if (peers.ContainsKey(currentMessage.a))
//                 {
//                     // OnPeerFound?.Invoke(currentMessage.f, long.Parse(currentMessage.d));
//                     peers[currentMessage.a].age = Time.realtimeSinceStartup;
//                 }
//                 break;
// 
//             case NetworkMessageType.HeartbeatMessage:
//                 //new peer:
//                 if (!peers.ContainsKey(currentMessage.a))
//                 {
//                     peers.Add(currentMessage.a, new TransmissionManager.Peer(currentMessage.a, currentMessage.f, 0, currentMessage.pl));
// 
//                 }
//                 //catalog heartbeat time:
//                 peers[currentMessage.a].age = Time.realtimeSinceStartup;
//                 break;
// 
//             case NetworkMessageType.QuaternionMessage:
//                 // Debug.Log("Quaternion Message Received");
//                 QuaternionMessage receivedQuaternionMessage = NetworkUtilities.UnpackMessage<QuaternionMessage>(rawMessage);
//                 TransmissionManager.instance.globalQuaternions[receivedQuaternionMessage.l] = receivedQuaternionMessage.q;
//                 break;
// 
//             case NetworkMessageType.FloatMessage:
//                 // Debug.Log("Float Message Received");
//                 FloatMessage receivedFloatMessage = NetworkUtilities.UnpackMessage<FloatMessage>(rawMessage);
//                 TransmissionManager.instance.globalFloats[receivedFloatMessage.l] = receivedFloatMessage.fl;
//                 break;
// 
//             case NetworkMessageType.CommandMessage:
//                 CommandMessage receivedCommandMessage = NetworkUtilities.UnpackMessage<CommandMessage>(rawMessage);
//                 // Debug.Log(receivedCommandMessage == null);
//                 // Debug.Log(receivedCommandMessage.ToString());
//                 // Debug.Log(receivedCommandMessage.f == null);
//                 // Debug.Log(receivedCommandMessage.co == null);
//                 // Debug.Log(receivedCommandMessage.co.ToString());
//                 HandleCommand(receivedCommandMessage.co);
//                 break;
// 
//             case NetworkMessageType.BaseControlMessage:
//                 BaseControlMessage receivedBaseControlMessage = NetworkUtilities.UnpackMessage<BaseControlMessage>(rawMessage);
//                 HandleBaseControl(receivedBaseControlMessage.bc);
//                 break;
//         }
//     }
// 
//     
// }
