using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(TransmissionManager))]
public class NetworkMessageHandler : MonoBehaviour
{
    // Streaming
    public Action<StreamDataHeader, byte[]> streamUpdated;

    void Start()
    {
        TransmissionManager.instance.moveRequestReceived += ExecuteMove;
    }

    private void OnDestroy()
    {
        TransmissionManager.instance.moveRequestReceived -= ExecuteMove;
    }


    void ExecuteMove(string siteEnumStr)
    {
        Debug.Log("Move request received");
        Transform targetTransform = SiteManager.instance.GetTransform(siteEnumStr);
        NavigationManager.instance.GoToTarget(targetTransform, siteEnumStr);
    }

    public void ProcessRawMessage(string rawMessage)
    {
        TransmissionManager transmissionManager = GetComponent<TransmissionManager>();
        // get message:
        NetworkMessage currentMessage = JsonUtility.FromJson<NetworkMessage>(rawMessage);

        // debug:
        if (transmissionManager.debugIncoming)
        {
            Debug.Log($"Received {rawMessage} from {currentMessage.f}");
        }

        var peers = transmissionManager.GetPeers();

        switch ((NetworkMessageType)currentMessage.ty)
        {
            case NetworkMessageType.StreamMessage:
                StreamMessage receivedStreamMessage = NetworkUtilities.UnpackMessage<StreamMessage>(rawMessage);
                StreamDataHeader receivedHeader = JsonUtility.FromJson<StreamDataHeader>(receivedStreamMessage.d);
                streamUpdated?.Invoke(receivedHeader, receivedStreamMessage.v);
                break;

            case NetworkMessageType.AwakeMessage:
                //if this peer hasn't been gone long then fire a recap:
                if (peers.ContainsKey(currentMessage.a))
                {
                    // OnPeerFound?.Invoke(currentMessage.f, long.Parse(currentMessage.d));
                    peers[currentMessage.a].age = Time.realtimeSinceStartup;
                }
                break;

            case NetworkMessageType.HeartbeatMessage:
                //new peer:
                if (!peers.ContainsKey(currentMessage.a))
                {
                    peers.Add(currentMessage.a, new TransmissionManager.Peer(currentMessage.a, currentMessage.f, 0, currentMessage.pl));

                }
                //catalog heartbeat time:
                peers[currentMessage.a].age = Time.realtimeSinceStartup;
                break;

            case NetworkMessageType.QuaternionMessage:
                // Debug.Log("Quaternion Message Received");
                QuaternionMessage receivedQuaternionMessage = NetworkUtilities.UnpackMessage<QuaternionMessage>(rawMessage);
                TransmissionManager.instance.globalQuaternions[receivedQuaternionMessage.l] = receivedQuaternionMessage.q;
                break;

            case NetworkMessageType.FloatMessage:
                // Debug.Log("Float Message Received");
                FloatMessage receivedFloatMessage = NetworkUtilities.UnpackMessage<FloatMessage>(rawMessage);
                TransmissionManager.instance.globalFloats[receivedFloatMessage.l] = receivedFloatMessage.fl;
                break;

            case NetworkMessageType.CommandMessage:
                CommandMessage receivedCommandMessage = NetworkUtilities.UnpackMessage<CommandMessage>(rawMessage);
                // Debug.Log(receivedCommandMessage == null);
                // Debug.Log(receivedCommandMessage.ToString());
                // Debug.Log(receivedCommandMessage.f == null);
                // Debug.Log(receivedCommandMessage.co == null);
                // Debug.Log(receivedCommandMessage.co.ToString());
                HandleCommand(receivedCommandMessage.co);
                break;


                // case NetworkMessageType.MoveRequestMessage:
                //     MoveRequestMessage moveRequestMessage = UnpackMessage<MoveRequestMessage>(rawMessage);
                //     moveRequestReceived?.Invoke(moveRequestMessage.se);
                //     break;
                // 
                // case NetworkMessageType.YieldControlMessage:
                //     YieldControlMessage yieldControlMessage = UnpackMessage<YieldControlMessage>(rawMessage);
                //     if (platform == yieldControlMessage.tp)
                //     {
                //         if (platform == PlatformEnum.MagicLeap)
                //         {
                //             CameraSwitch.SwitchCamera();
                //         }
                //     }
                //     break;
        }
    }

    void HandleCommand(Command command)
    {
        switch (command.type)
        {
            case CommandType.Grab:
                Debug.Log("Grab Command Received.");
                if (RoboyComponentsAccess.instance.roboyHands.isLeftHandGrabbing)
                {
                    RoboyComponentsAccess.instance.roboyHands.LeftRelease();
                }
                else
                {
                    RoboyComponentsAccess.instance.roboyHands.LeftGrab();
                }
                break;

            case CommandType.Displace:
                Debug.Log($"Target Location: {command.targetLocation}");
                NavigationManager.instance.SetDestination(command.targetLocation);
                break;

            case CommandType.Patrol:
                if (command.waypoints.Length == 0)
                {
                    NavigationManager.instance.StopPatrol();
                }
                else
                {
                    NavigationManager.instance.StartPatrol(command.waypoints);
                }
                break;

            default:
                break;
        }
    }
}
