using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum NetworkMessageType
{
    GlobalStringsRequestMessage,
    GlobalFloatsRequestMessage,
    GlobalBoolsRequestMessage,
    GlobalVector2RequestMessage,
    GlobalVector3RequestMessage,
    GlobalVector4RequestMessage,
    HeartbeatMessage,
    ConfirmedMessage,
    AwakeMessage,
    DespawnMessage,
    GlobalBoolChangedMessage,
    GlobalBoolsRecapMessage,
    GlobalFloatChangedMessage,
    GlobalFloatsRecapMessage,
    GlobalStringChangedMessage,
    GlobalStringsRecapMessage,
    GlobalVector2ChangedMessage,
    GlobalVector2RecapMessage,
    GlobalVector3ChangedMessage,
    GlobalVector3RecapMessage,
    GlobalVector4ChangedMessage,
    GlobalVector4RecapMessage,
    OnDisabledMessage,
    OnEnabledMessage,
    OwnershipTransferenceDeniedMessage,
    OwnershipTransferenceGrantedMessage,
    OwnershipTransferenceRequestMessage,
    SpawnMessage,
    SpawnRecapMessage,
    TransformSyncMessage,
    BoolArrayMessage,
    BoolMessage,

    ColorArrayMessage,
    ColorMessage,
    FloatArrayMessage,

    PoseArrayMessage,
    PoseMessage,
    QuaternionArrayMessage,
    RPCMessage,
    StringArrayMessage,
    StringMessage,
    Vector2ArrayMessage,
    Vector2Message,
    Vector3ArrayMessage,

    Vector4ArrayMessage,
    Vector4Message,
    SpatialAlignmentMessage,

    StreamRequestMessage,
    StreamMessage,
    MoveRequestMessage,
    TransformMessage,

    ByteArrayMessage,
    Vector3Message,
    QuaternionMessage,
    FloatMessage,

    CommandMessage,
    OperationMessage,

    NotificationMessage,
}


public class NetworkMessage : Message
{
    // Public Variables (truncated to reduce packet size):

    /// <summary>
    /// to
    /// </summary>
    public string t;
    /// <summary>
    /// from
    /// </summary>
    public string f;
    /// <summary>
    /// guid
    /// </summary>
    public string g;
    /// <summary>
    /// reliable
    /// </summary>
    public int r;
    /// <summary>
    /// targets
    /// </summary>
    public int ts;
    /// <summary>
    /// time
    /// </summary>
    public double ti;
    /// <summary>
    /// data
    /// </summary>
    public string d;
    /// <summary>
    /// type
    /// </summary>
    public short ty;
    /// <summary>
    /// appKey
    /// </summary>
    public string a;
    /// <summary>
    /// privatekey
    /// </summary>
    public string p;

    //Constructors:
    public NetworkMessage(NetworkMessageType type, NetworkAudience audience = NetworkAudience.NetworkBroadcast, string targetAddress = "", bool reliable = false, string data = "", string guid = "")
    {
        switch (audience)
        {
            case NetworkAudience.SinglePeer:
                r = reliable ? 1 : 0;
                t = targetAddress;
                break;

            case NetworkAudience.KnownPeers:
                r = reliable ? 1 : 0;
                t = "";
                break;

            case NetworkAudience.NetworkBroadcast:
                // r = 0;
                r = reliable ? 1 : 0;
                t = "255.255.255.255";
                break;
        }

        //guids are only required for reliable messages:
        if (reliable && string.IsNullOrEmpty(guid))
        {
            g = Guid.NewGuid().ToString();
        }
        else
        {
            g = guid;
        }

        f = NetworkUtilities.MyAddress;
        ti = Math.Round(Time.realtimeSinceStartup, 3);
        d = data;
        ty = (short)type;
        a = ""; // TransmissionManager.Instance.appKey;
        p = ""; // TransmissionManager.Instance.privateKey;
    }

    public override string ProduceString()
    {
        return JsonUtility.ToJson(this);
    }

    public override byte[] ProduceBytes()
    {
        return Encoding.UTF8.GetBytes(ProduceString());
    }
}


public class ByteArrayMessage : NetworkMessage
{
    //Public Variables(truncated to reduce packet size):
    /// <summary>
    /// values
    /// </summary>
    public byte[] v;

    //Constructors:
    public ByteArrayMessage(byte[] values, string data = "", NetworkAudience audience = NetworkAudience.NetworkBroadcast, string targetAddress = "")
        : base(NetworkMessageType.ByteArrayMessage, audience, targetAddress, false, data)
    {
        v = values;
    }
}

public class StreamMessage : ByteArrayMessage
{
    //Constructors:
    public StreamMessage(StreamDataHeader header, byte[] values) : base(values)
    {
        ty = (short)NetworkMessageType.StreamMessage;
        d = JsonUtility.ToJson(header);
        v = values;
    }
}

// public class StreamMessage : NetworkMessage
// {
//     //Constructors:
//     public StreamMessage(StreamFrame streamFrame, string data = "", NetworkAudience audience = NetworkAudience.NetworkBroadcast, string targetAddress = "")
//         : base(NetworkMessageType.StreamMessage, audience, targetAddress, true, data)
//     {
//         d = JsonUtility.ToJson(streamFrame);
//     }
// }

public class Vector3Message : NetworkMessage
{
    /// <summary>
    /// Label (name) of the quaternion
    /// </summary>
    public string l;

    /// <summary>
    /// The quaternion to be transmitted
    /// </summary>
    public Vector3 v;

    public Vector3Message(string a_label, Vector3 a_vector3, string data = "", NetworkAudience audience = NetworkAudience.NetworkBroadcast, string targetAddress = "")
        : base(NetworkMessageType.Vector3Message, audience, targetAddress, false, data)
    {
        l = a_label;
        v = a_vector3;
    }
}

public class QuaternionMessage : NetworkMessage
{
    /// <summary>
    /// Label (name) of the quaternion
    /// </summary>
    public string l;

    /// <summary>
    /// The quaternion to be transmitted
    /// </summary>
    public Quaternion q;

    public QuaternionMessage(string a_label, Quaternion a_quaternion, string data = "", NetworkAudience audience = NetworkAudience.NetworkBroadcast, string targetAddress = "")
        : base(NetworkMessageType.QuaternionMessage, audience, targetAddress, false, data)
    {
        l = a_label;
        q = a_quaternion;
    }
}

public class FloatMessage : NetworkMessage
{
    /// <summary>
    /// Label (name) of the float
    /// </summary>
    public string l;

    /// <summary>
    /// The float to be transmitted
    /// </summary>
    public float fl;

    public FloatMessage(string a_label, float a_float, string data = "", NetworkAudience audience = NetworkAudience.NetworkBroadcast, string targetAddress = "")
        : base(NetworkMessageType.FloatMessage, audience, targetAddress, false, data)
    {
        l = a_label;
        fl = a_float;
    }

}

public class CommandMessage : NetworkMessage
{
    /// <summary>
    /// The type of the command
    /// </summary>
    public Command co;

    public CommandMessage(Command a_command, string data = "", NetworkAudience audience = NetworkAudience.NetworkBroadcast, string targetAddress = "")
        : base(NetworkMessageType.CommandMessage, audience, targetAddress, true, data)
    {
        co = a_command;
    }
}

public class OperationMessage : NetworkMessage
{
    public Operation o;
    public OperationMessage(Operation a_operation, string data = "", NetworkAudience audience = NetworkAudience.NetworkBroadcast, string targetAddress = "")
        : base(NetworkMessageType.OperationMessage, audience, targetAddress, true, data)
    {
        o = a_operation;
    }
}
