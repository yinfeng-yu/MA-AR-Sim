using System;
using System.Collections.Generic;
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
    ByteArrayMessage,
    ColorArrayMessage,
    ColorMessage,
    FloatArrayMessage,
    FloatMessage,
    PoseArrayMessage,
    PoseMessage,
    QuaternionArrayMessage,
    QuaternionMessage,
    RPCMessage,
    StringArrayMessage,
    StringMessage,
    Vector2ArrayMessage,
    Vector2Message,
    Vector3ArrayMessage,
    Vector3Message,
    Vector4ArrayMessage,
    Vector4Message,
    SpatialAlignmentMessage,

    StreamRequestMessage,
    StreamMessage,
    MoveRequestMessage,
    YieldControlMessage,
}

public class NetworkMessage
{
    //Public Variables (truncated to reduce packet size):
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
    /// <summary>
    /// platform
    /// </summary>
    public PlatformEnum pl;

    //Constructors:
    public NetworkMessage(NetworkMessageType type, NetworkAudience audience, string targetAddress = "", bool reliable = false, string data = "", string guid = "")
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
                r = 0;
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


        f = MyNetworkUtilities.MyAddress;
        ti = Math.Round(Time.realtimeSinceStartup, 3);
        d = data;
        ty = (short)type;
        a = MyTransmission.instance.appKey;
        p = MyTransmission.instance.privateKey;
        pl = MyTransmission.instance.platform;
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
    public ByteArrayMessage(byte[] values, string data = "", NetworkAudience audience = NetworkAudience.NetworkBroadcast, string targetAddress = "") : base(NetworkMessageType.ByteArrayMessage, audience, targetAddress, true, data)
    {
        v = values;
    }
}

public class StreamRequestMessage : NetworkMessage
{
    /// <summary>
    /// Camera view
    /// </summary>
    public int vi;

    public StreamRequestMessage(int view, string data = "", NetworkAudience audience = NetworkAudience.NetworkBroadcast, string targetAddress = "") : base(NetworkMessageType.StreamRequestMessage, audience, targetAddress, true, data)
    {
        vi = view;
    }
}

public class StreamMessage : ByteArrayMessage
{
    //Constructors:
    public StreamMessage(StreamChunkMetaData metadata, byte[] values) : base(values)
    {
        ty = (short)NetworkMessageType.StreamMessage;
        d = JsonUtility.ToJson(metadata);
        v = values;
    }
}

public class MoveRequestMessage : NetworkMessage
{
    public string se;

    public MoveRequestMessage(string siteEnumStr, string data = "", NetworkAudience audience = NetworkAudience.NetworkBroadcast, string targetAddress = "") : base(NetworkMessageType.MoveRequestMessage, audience, targetAddress, true, data)
    {
        se = siteEnumStr;
    }
}

public class YieldControlMessage : NetworkMessage
{
    public PlatformEnum tp;

    public YieldControlMessage(PlatformEnum targetPlatform, string data = "", NetworkAudience audience = NetworkAudience.NetworkBroadcast, string targetAddress = "") : base(NetworkMessageType.YieldControlMessage, audience, targetAddress, true, data)
    {
        tp = targetPlatform;
    }
}