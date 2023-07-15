#define UNITY_DEBUG

using UnityEngine;
using UnityEngine.Events;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

public enum NetworkAudience { SinglePeer, KnownPeers, NetworkBroadcast };

public enum PlatformEnum { Unknown, Touchscreen, MagicLeap };

public struct StreamDataChunk
{
    public string token;
    public int index;
    public int dataSize;
    public int frameSize;
    public int frameWidth;
    public int frameHeight;
    public int numFrameChunks;
    public string data;
}

public struct StreamChunkMetaData
{
    public int index;
    public int frameSize;
    public int frameWidth;
    public int frameHeight;
    public int numFrameChunks;
    public int offset;
}

public class MyTransmission : MonoBehaviour
{
    //Public Variables:
    public int port = 23000;

    // For Unity Editor debug use
    public int sendPort = 23001;
    public int receivePort = 23002;

    public int bufferSize = 2048;
    // [Tooltip("On component addition a randomized ID will be generated.  All applications running on your network must have the same appKey and privateKey to recognize eachother - empty keys are accepted.")]
    [Tooltip("On component addition a randomized ID will be generated.  All applications running on your network must have a unique appKey as the ID - empty keys are accepted.")]
    public string appKey;
    // [Tooltip("All applications running on your network must have the same appKey and privateKey to recognize eachother - empty keys are accepted.")]
    public string privateKey;
    [Tooltip("All GameObjects in this list (in addition to the Transmission GameObject) will receive SendMessages when RPC messages are sent.")]
    public GameObject[] rpcTargets;
    public Pose sharedOrigin;
    public bool debugOutgoing;
    public bool debugIncoming;
    public static DateTime startUpTime;

    public PlatformEnum platform;

    // Streaming
    public List<string> streamTokens = new List<string>();
    public int availableChunkSize = 512;
    public int numFrameChunks;

    public byte[] _streamData;
    public Action<StreamChunkMetaData, byte[]> streamUpdated;

    // Move Request
    public Action<string> moveRequestReceived;

    //Public Properties:
    public static MyTransmission instance
    {
        get
        {
            if (_quitting)
            {
                return null;
            }

            //find:
            if (_instance == null)
            {
                _instance = FindObjectOfType<MyTransmission>();
            }

            //missing:
            if (_instance == null)
            {
                Debug.Log("No instance of Transmission found in scene.");
            }

            return _instance;
        }
    }

    public string[] Peers
    {
        get
        {
            return _peers.Keys.ToArray();
        }
    }

    public string OldestPeer
    {
        get;
        private set;
    }

    //Private Variables:
    private const float HeartbeatInterval = 2;
    private const float ReliableResendInterval = .5f;
    private const float MaxResendDuration = 7;
    private const float _stalePeerTimeout = 8;
    private const float OldestIdentifierTimeout = 3;
    private static bool _receiveThreadAlive;
    private static ConcurrentBag<string> _receivedMessages = new ConcurrentBag<string>(); //do we need to be concerned about the constant growth of this?
    private List<string> _confirmedReliableMessages = new List<string>();
    private static Dictionary<string, NetworkMessage> _unconfirmedReliableMessages = new Dictionary<string, NetworkMessage>();

    [Serializable] 
    public class Peer
    {
        public string key;
        public string ipAddress;
        public float age;
        public PlatformEnum platform;

        public Peer(string _key, string _ipAddress, float _age, PlatformEnum _platform)
        {
            key = _key;
            ipAddress = _ipAddress;
            age = _age;
            platform = _platform;
        }
    }

    // private static Dictionary<string, float> _peers = new Dictionary<string, float>();
    private static Dictionary<string, Peer> _peers = new Dictionary<string, Peer>();

    private static MyTransmission _instance;
    private static UdpClient _udpClient;
    private static Thread _receiveThread;
    private static IPEndPoint _receiveEndPoint = new IPEndPoint(IPAddress.Any, 0);
    private static bool _initialized;

    private static Dictionary<string, string> _globalStrings = new Dictionary<string, string>();
    private static Dictionary<string, float> _globalFloats = new Dictionary<string, float>();
    private static Dictionary<string, bool> _globalBools = new Dictionary<string, bool>();
    private static Dictionary<string, Vector2> _globalVector2 = new Dictionary<string, Vector2>();
    private static Dictionary<string, Vector3> _globalVector3 = new Dictionary<string, Vector3>();
    private static Dictionary<string, Vector4> _globalVector4 = new Dictionary<string, Vector4>();

    // private static Dictionary<string, List<TransmissionObject>> _spawnedObjects = new Dictionary<string, List<TransmissionObject>>();
    // private static SortedDictionary<long, string> _peerAges = new SortedDictionary<long, string>();
    private static bool _quitting;
    private Pose _previousSharedOrigin;
    private static long _age;

    //Init:
    private void Awake()
    {
        _age = DateTime.Now.Ticks;
        Initialize();
    }

    private void Reset()
    {
        appKey = MyNetworkUtilities.UniqueID();
    }

    //Deinit:
    private void OnApplicationQuit()
    {
        _quitting = true;
    }

    private void OnDestroy()
    {
        //stop receive thread:
        if (_receiveThread != null)
        {
            _receiveThread.Abort();
        }
        _receiveThreadAlive = false;

        //close socket:
        if (_udpClient != null)
        {
            _udpClient.Close();
        }

        StopAllCoroutines();
    }

    //Loops:
    private void Update()
    {
        ReceiveMessages();

        //respond to shared origin updates:
        if (_previousSharedOrigin != sharedOrigin)
        {
            _previousSharedOrigin = sharedOrigin;
            // TransmissionObject.SynchronizeAll();
            // OnSharedOriginUpdated?.Invoke(sharedOrigin);
        }
    }

    /// <summary>
    /// Checks if a known peer still has an active heartbeat.
    /// </summary>
    public static bool PeerAlive(string address)
    {
        // TODO
        return _peers.ContainsKey(address);
    }

    public List<Peer> GetPeers()
    {
        List<Peer> outputList = new List<Peer>();
        foreach (var item in _peers.ToList())
        {
            outputList.Add(item.Value);
        }
        return outputList;
    }

    /// <summary>
    /// Transmits a NetworkMessage to the network.
    /// </summary>
    public static void Send(NetworkMessage message)
    {
        // reliable logging:
        if (message.r == 1)
        {
            if (!_unconfirmedReliableMessages.ContainsKey(message.g))
            {
                // set target counts:
                if (string.IsNullOrEmpty(message.t))
                {
                    message.ts = _peers.Count;
                }
                else
                {
                    message.ts = 1;
                }

                _unconfirmedReliableMessages.Add(message.g, message);
            }
        }

        //generate transmission:
        string serialized = JsonUtility.ToJson(message);
        byte[] bytes = Encoding.UTF8.GetBytes(serialized);
        
#if UNITY_DEBUG
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, instance.sendPort);
        // _udpClient = new UdpClient(instance.receivePort);
#else
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, instance.port);
#endif

        //size check:
        if (bytes.Length > _udpClient.Client.SendBufferSize)
        {
            Debug.Log($"Message too large to send! Buffer is currently {instance.bufferSize} bytes and you are tring to send {bytes.Length} bytes. Try increasing the buffer size.");
            return;
        }

        //send:
        if (string.IsNullOrEmpty(message.t))
        {
            //send to all peers:
            foreach (var item in _instance.Peers)
            {
                endPoint.Address = IPAddress.Parse(item);
                _udpClient.Send(bytes, bytes.Length, endPoint);

                //debug:
                if (instance.debugOutgoing)
                {
                    Debug.Log($"Sent {serialized} to {endPoint.Address.ToString()}");
                }
            }
        }
        else
        {
            endPoint.Address = IPAddress.Parse(message.t);
            _udpClient.Send(bytes, bytes.Length, endPoint);

            //debug:
            if (instance.debugOutgoing)
            {
                Debug.Log($"Sent {serialized} to {endPoint}");
            }
        }
    }

    private void Initialize()
    {
        //flag initializtion complete:
        if (_initialized)
        {
            return;
        }
        _initialized = true;

        //establish socket:
        bool socketOpen = false;
        while (!socketOpen)
        {
            try
            {
#if UNITY_DEBUG
                _udpClient = new UdpClient(instance.receivePort);
#else
                _udpClient = new UdpClient(instance.port);
#endif

                _udpClient.Client.SendBufferSize = instance.bufferSize;
                _udpClient.Client.ReceiveBufferSize = instance.bufferSize;
                socketOpen = true;
            }
            catch (Exception)
            {
            }
        }

        //establish receive thread:
        _receiveThreadAlive = true;
        _receiveThread = new Thread(new ThreadStart(Receive));
        _receiveThread.IsBackground = true;
        _receiveThread.Start();

        numFrameChunks = _streamData.Length / availableChunkSize + (_streamData.Length % availableChunkSize == 0 ? 0 : 1);

        //fire off an awake event:
        Send(new NetworkMessage(NetworkMessageType.AwakeMessage, NetworkAudience.NetworkBroadcast, "", true, _age.ToString()));

        instance.StartCoroutine(Heartbeat());
        instance.StartCoroutine(ReliableRetry());

    }

    private void ReceiveMessages()
    {
        while (_receivedMessages.Count > 0)
        {
            string rawMessage;
            if (_receivedMessages.TryTake(out rawMessage))
            {
                //get message:
                NetworkMessage currentMessage = JsonUtility.FromJson<NetworkMessage>(rawMessage);

                //debug:
                if (debugIncoming)
                {
                    Debug.Log($"Received {rawMessage} from {currentMessage.f}");
                }

                //parse status:
                bool needToParse = true;

                //reliable message?
                if (currentMessage.r == 1)
                {
                    if (_confirmedReliableMessages.Contains(currentMessage.g))
                    {
                        //we have previously consumed this message but the confirmation failed so we only
                        //need to focus on sending another confirmation:
                        needToParse = false;
                        continue;
                    }
                    else
                    {
                        //mark this reliable message as confirmed:
                        _confirmedReliableMessages.Add(currentMessage.g);
                    }

                    //send back confirmation message with same guid:
                    NetworkMessage confirmationMessage = new NetworkMessage(
                        NetworkMessageType.ConfirmedMessage,
                        NetworkAudience.SinglePeer,
                        currentMessage.f,
                        false,
                        "",
                        currentMessage.g);

                    Send(confirmationMessage);
                }

                //parsing needed?
                if (!needToParse)
                {
                    continue;
                }

                switch ((NetworkMessageType)currentMessage.ty)
                {
                    case NetworkMessageType.StreamRequestMessage:
                        // Debug.Log("received stream request");
                        StreamRequestMessage receivedStreamRequestMessage = UnpackMessage<StreamRequestMessage>(rawMessage);
                        _streamData = TextureLoader.instance.GetSerializedRenderTexture(receivedStreamRequestMessage.vi);

                        numFrameChunks = _streamData.Length / availableChunkSize + (_streamData.Length % availableChunkSize == 0 ? 0 : 1);

                        for (int i = 0; i < numFrameChunks; i++)
                        {
                            int thisChunkSize = availableChunkSize;
                            if (i == numFrameChunks - 1)
                            {
                                thisChunkSize = _streamData.Length % availableChunkSize == 0 ? availableChunkSize : _streamData.Length % availableChunkSize;
                            }

                            StreamChunkMetaData streamChunkMetaData;
                            streamChunkMetaData.index = i;
                            streamChunkMetaData.frameSize = _streamData.Length;
                            streamChunkMetaData.frameWidth = TextureLoader.instance.fpSrc.width; // doesn't matter: fp_src and tp_src have the same dimension
                            streamChunkMetaData.frameHeight = TextureLoader.instance.fpSrc.height;
                            streamChunkMetaData.numFrameChunks = numFrameChunks;
                            streamChunkMetaData.offset = i * availableChunkSize;

                            byte[] chunk = new byte[thisChunkSize];

                            Buffer.BlockCopy(_streamData, i * availableChunkSize, chunk, 0, thisChunkSize);
                            
                            Send(new StreamMessage(streamChunkMetaData, chunk));
                        }

                        break;

                    case NetworkMessageType.StreamMessage:

                        StreamMessage receivedStreamMessage = UnpackMessage<StreamMessage>(rawMessage);
                        StreamChunkMetaData receivedMetaData = JsonUtility.FromJson<StreamChunkMetaData>(receivedStreamMessage.d);
                        byte[] frameChunk = receivedStreamMessage.v;

                        streamUpdated?.Invoke(receivedMetaData, frameChunk);
                        break;

                    case NetworkMessageType.AwakeMessage:
                        //if this peer hasn't been gone long then fire a recap:
                        if (_peers.ContainsKey(currentMessage.a))
                        {
                            // OnPeerFound?.Invoke(currentMessage.f, long.Parse(currentMessage.d));
                            _peers[currentMessage.a].age = Time.realtimeSinceStartup;
                        }
                        break;

                    case NetworkMessageType.HeartbeatMessage:
                        //new peer:
                        if (!_peers.ContainsKey(currentMessage.a))
                        {
                            _peers.Add(currentMessage.a, new Peer(currentMessage.a, currentMessage.f, 0, currentMessage.pl));
                                            
                        }
                        //catalog heartbeat time:
                        _peers[currentMessage.a].age = Time.realtimeSinceStartup;
                        break;

                    case NetworkMessageType.MoveRequestMessage:
                        MoveRequestMessage moveRequestMessage = UnpackMessage<MoveRequestMessage>(rawMessage);
                        moveRequestReceived?.Invoke(moveRequestMessage.se);
                        break;

                    case NetworkMessageType.YieldControlMessage:
                        YieldControlMessage yieldControlMessage = UnpackMessage<YieldControlMessage>(rawMessage);
                        if (platform == yieldControlMessage.tp)
                        {
                            if (platform == PlatformEnum.MagicLeap)
                            {
                                CameraSwitch.SwitchCamera();
                            }
                        }
                        break;
                }
            }
        }
    }

    private T UnpackMessage<T>(string rawMessage)
    {
        return JsonUtility.FromJson<T>(rawMessage);
    }

    private static IEnumerator ReliableRetry()
    {
        while (true)
        {
            //iterate a copy so we don't have issues with inbound confirmations:
            foreach (var item in _unconfirmedReliableMessages.Values.ToArray())
            {
                if (Time.realtimeSinceStartup - item.ti < MaxResendDuration)
                {
                    //resend:
                    Send(item);
                }
                else
                {
                    //TODO: add explict list of who didn't get it for KnownPeers intended messages:
                    //reliable message send failed - only if we have some targets left, otherwise there 
                    //were no recipients to begin with which easily happens if someone attempted a KnownPeers
                    //send when no one was around:
                    if (item.ts != 0)
                    {
                        // instance.OnSendMessageFailure?.Invoke(item.g);
                    }
                    _unconfirmedReliableMessages.Remove(item.g);
                }
            }

            //loop:
            yield return new WaitForSeconds(ReliableResendInterval);
            yield return null;
        }
    }

    private static IEnumerator Heartbeat()
    {
        while (true)
        {
            //transmit message - set startup time as our data for oldest peer evaluations:
            Send(new NetworkMessage(NetworkMessageType.HeartbeatMessage, NetworkAudience.NetworkBroadcast, "", false, _age.ToString()));

            //stale peer identification:
            List<string> stalePeers = new List<string>();
            foreach (var item in _peers.ToList())
            {
                if (Time.realtimeSinceStartup - item.Value.age > _stalePeerTimeout)
                {
                    stalePeers.Add(item.Key);
                }
            }
            
            //stale peer removal:
            foreach (var item in stalePeers)
            {
                _peers.Remove(item);
            }

            //loop:
            yield return new WaitForSeconds(HeartbeatInterval);
            yield return null;
        }
    }

    //Threads:
    private static void Receive()
    {
        while (_receiveThreadAlive)
        {
            if (instance == null)
            {
                break;
            }

            byte[] bytes = _udpClient.Receive(ref _receiveEndPoint);

            //get raw message for key evaluation:
            string serialized = Encoding.UTF8.GetString(bytes);

            NetworkMessage rawMessage = JsonUtility.FromJson<NetworkMessage>(serialized);

            //keys evaluations:
            if (rawMessage.a != instance.appKey)
            {
                //we send the serialized string for easier debug messages:
                _receivedMessages.Add(serialized);
            }

        }
    }

    // Custom requests
    public void SendStreamRequest(int viewType)
    {
        Send(new StreamRequestMessage(viewType));
    }

    public void SendMoveRequest(SiteEnum a_siteEnum)
    {
        Send(new MoveRequestMessage(a_siteEnum.ToString()));
        Debug.Log("Move request sent");
    }

    public void SendYieldControlMessage(PlatformEnum a_platform)
    {
        Send(new YieldControlMessage(a_platform));
    }
}
