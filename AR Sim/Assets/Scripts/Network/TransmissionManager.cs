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

public class TransmissionManager : MonoBehaviour
{
    //Public Variables:
    public int port = 23000;

    // For Unity Editor debug use
    public int sendPort = 23001;
    public int receivePort = 23002;

    public int bufferSize = 1024;
    [Tooltip("On component addition a randomized ID will be generated.  All applications running on your network must have the same appKey and privateKey to recognize eachother - empty keys are accepted.")]
    public string appKey;
    [Tooltip("All applications running on your network must have the same appKey and privateKey to recognize eachother - empty keys are accepted.")]
    public string privateKey;
    [Tooltip("All GameObjects in this list (in addition to the Transmission GameObject) will receive SendMessages when RPC messages are sent.")]
    public GameObject[] rpcTargets;
    public Pose sharedOrigin;
    public bool debugOutgoing;
    public bool debugIncoming;
    public static DateTime startUpTime;

    //Public Properties:
    public static TransmissionManager instance
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
                _instance = FindObjectOfType<TransmissionManager>();
            }

            //missing:
            if (_instance == null)
            {
                Debug.Log("No instance of Transmission found in scene.");
            }

            //initialize:
            Initialize();

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
    private const float StalePeerTimeout = 8;
    private const float OldestIdentifierTimeout = 3;
    private static bool _receiveThreadAlive;
    private static ConcurrentBag<string> _receivedMessages = new ConcurrentBag<string>(); //do we need to be concerned about the constant growth of this?
    private List<string> _confirmedReliableMessages = new List<string>();
    private static Dictionary<string, NetworkMessage> _unconfirmedReliableMessages = new Dictionary<string, NetworkMessage>();

    private static Dictionary<string, float> _peers = new Dictionary<string, float>();
    [SerializeField] private List<string> _peekPeers = new List<string>();

    private static TransmissionManager _instance;
    private static UdpClient _udpClient;
    private static Thread _receiveThread;
    private static IPEndPoint _receiveEndPoint = new IPEndPoint(IPAddress.Any, 0);
    private static bool _initialized;
    private static Coroutine _alignmentCoroutine;
    private static Dictionary<string, string> _globalStrings = new Dictionary<string, string>();
    private static Dictionary<string, float> _globalFloats = new Dictionary<string, float>();
    private static Dictionary<string, bool> _globalBools = new Dictionary<string, bool>();
    private static Dictionary<string, Vector2> _globalVector2 = new Dictionary<string, Vector2>();
    private static Dictionary<string, Vector3> _globalVector3 = new Dictionary<string, Vector3>();
    private static Dictionary<string, Vector4> _globalVector4 = new Dictionary<string, Vector4>();
    // private static Dictionary<string, List<TransmissionObject>> _spawnedObjects = new Dictionary<string, List<TransmissionObject>>();
    private static SortedDictionary<long, string> _peerAges = new SortedDictionary<long, string>();
    private static bool _quitting;
    private Pose _previousSharedOrigin;
    private static long _age;

    //Init:
    private void Awake()
    {
        _age = DateTime.Now.Ticks;
        _peerAges.Add(_age, MyNetworkUtilities.MyAddress);
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
        return _peers.ContainsKey(address);
    }

    /// <summary>
    /// Transmits a NetworkMessage to the network.
    /// </summary>
    public static void Send(NetworkMessage message)
    {
        //reliable logging:
        if (message.r == 1)
        {
            if (!_unconfirmedReliableMessages.ContainsKey(message.g))
            {
                //set target counts:
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

    private static void Initialize()
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
                _udpClient = new UdpClient(instance.sendPort);
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
                    //     case NetworkMessageType.GlobalStringsRequestMessage:
                    //         Send(new GlobalStringsRecapMessage(currentMessage.f, _globalStrings.Keys.ToArray<string>(), _globalStrings.Values.ToArray<string>()));
                    //         break;
                    // 
                    //     case NetworkMessageType.GlobalFloatsRequestMessage:
                    //         Send(new GlobalFloatsRecapMessage(currentMessage.f, _globalFloats.Keys.ToArray<string>(), _globalFloats.Values.ToArray<float>()));
                    //         break;
                    // 
                    //     case NetworkMessageType.GlobalBoolsRequestMessage:
                    //         string boolsSerialized = JsonUtility.ToJson(_globalBools);
                    //         Send(new GlobalBoolsRecapMessage(currentMessage.f, _globalBools.Keys.ToArray<string>(), _globalBools.Values.ToArray<bool>()));
                    //         break;
                    // 
                    //     case NetworkMessageType.GlobalVector2RequestMessage:
                    //         string vec2Serialized = JsonUtility.ToJson(_globalVector2);
                    //         Send(new GlobalVector2RecapMessage(currentMessage.f, _globalVector2.Keys.ToArray<string>(), _globalVector2.Values.ToArray<Vector2>()));
                    //         break;
                    // 
                    //     case NetworkMessageType.GlobalVector3RequestMessage:
                    //         string vec3Serialized = JsonUtility.ToJson(_globalVector3);
                    //         Send(new GlobalVector3RecapMessage(currentMessage.f, _globalVector3.Keys.ToArray<string>(), _globalVector3.Values.ToArray<Vector3>()));
                    //         break;
                    // 
                    //     case NetworkMessageType.GlobalVector4RequestMessage:
                    //         string vec4Serialized = JsonUtility.ToJson(_globalVector4);
                    //         Send(new GlobalVector4RecapMessage(currentMessage.f, _globalVector4.Keys.ToArray<string>(), _globalVector4.Values.ToArray<Vector4>()));
                    //         break;
                    // 
                    case NetworkMessageType.AwakeMessage:
                        //if this peer hasn't been gone long then fire a recap:
                        if (_peers.ContainsKey(currentMessage.f))
                        {
                            // OnPeerFound?.Invoke(currentMessage.f, long.Parse(currentMessage.d));
                            _peers[currentMessage.f] = Time.realtimeSinceStartup;
                        }
                        break;

                    case NetworkMessageType.HeartbeatMessage:
                        //new peer:
                        if (!_peers.ContainsKey(currentMessage.f))
                        {
                            _peers.Add(currentMessage.f, 0);

                            //oldest peer determination:
                            _peerAges.Add(long.Parse(currentMessage.d), currentMessage.f);
                            StopCoroutine("OldestIdentifier");
                            StartCoroutine("OldestIdentifier");

                            //if I have no global values then ask this new peer for theirs:
                            if (_peers.Count == 1)
                            {
                                if (_globalStrings.Count == 0)
                                {
                                    Send(new NetworkMessage(NetworkMessageType.GlobalStringsRequestMessage, NetworkAudience.SinglePeer, currentMessage.f, true));
                                }

                                if (_globalBools.Count == 0)
                                {
                                    Send(new NetworkMessage(NetworkMessageType.GlobalBoolsRequestMessage, NetworkAudience.SinglePeer, currentMessage.f, true));
                                }

                                if (_globalFloats.Count == 0)
                                {
                                    Send(new NetworkMessage(NetworkMessageType.GlobalFloatsRequestMessage, NetworkAudience.SinglePeer, currentMessage.f, true));
                                }

                                if (_globalVector2.Count == 0)
                                {
                                    Send(new NetworkMessage(NetworkMessageType.GlobalVector2RequestMessage, NetworkAudience.SinglePeer, currentMessage.f, true));
                                }

                                if (_globalVector3.Count == 0)
                                {
                                    Send(new NetworkMessage(NetworkMessageType.GlobalVector3RequestMessage, NetworkAudience.SinglePeer, currentMessage.f, true));
                                }

                                if (_globalVector4.Count == 0)
                                {
                                    Send(new NetworkMessage(NetworkMessageType.GlobalVector4RequestMessage, NetworkAudience.SinglePeer, currentMessage.f, true));
                                }
                            }

                            // OnPeerFound?.Invoke(currentMessage.f, _age);
                        }
                        //catalog heartbeat time:
                        _peers[currentMessage.f] = Time.realtimeSinceStartup;
                        break;
                        // 
                        //     case NetworkMessageType.ConfirmedMessage:
                        //         //update confirmed targets (this allows for KnownPeers confirmation from all
                        //         //peers):
                        //         _unconfirmedReliableMessages[currentMessage.g].ts -= 1;
                        // 
                        //         //all done?
                        //         if (_unconfirmedReliableMessages[currentMessage.g].ts == 0)
                        //         {
                        //             //confirmed!
                        //             _unconfirmedReliableMessages.Remove(currentMessage.g);
                        //             OnSendMessageSuccess?.Invoke(currentMessage.g);
                        //         }
                        //         break;
                        // 
                        //     case NetworkMessageType.RPCMessage:
                        //         RPCMessage rpcMessage = UnpackMessage<RPCMessage>(rawMessage);
                        // 
                        //         gameObject.SendMessage(rpcMessage.m, rpcMessage.pa, SendMessageOptions.DontRequireReceiver);
                        //         foreach (var item in rpcTargets)
                        //         {
                        //             item.SendMessage(rpcMessage.m, rpcMessage.pa, SendMessageOptions.DontRequireReceiver);
                        //         }
                        //         break;
                        // 
                        //     case NetworkMessageType.OnEnabledMessage:
                        //         OnEnabledMessage onEnabledMessage = UnpackMessage<OnEnabledMessage>(rawMessage);
                        // 
                        //         TransmissionObject enableTarget = TransmissionObject.Get(onEnabledMessage.ig);
                        //         if (enableTarget != null)
                        //         {
                        //             enableTarget.gameObject.SetActive(true);
                        //         }
                        // 
                        //         break;
                        // 
                        //     case NetworkMessageType.OnDisabledMessage:
                        //         OnDisabledMessage onDisabledMessage = UnpackMessage<OnDisabledMessage>(rawMessage);
                        // 
                        //         TransmissionObject disableTarget = TransmissionObject.Get(onDisabledMessage.ig);
                        //         if (disableTarget != null)
                        //         {
                        //             disableTarget.gameObject.SetActive(false);
                        //         }
                        //         break;
                        // 
                        //     case NetworkMessageType.GlobalFloatChangedMessage:
                        //         GlobalFloatChangedMessage globalFloatChangedMessage = UnpackMessage<GlobalFloatChangedMessage>(rawMessage);
                        //         SetLocalGlobalFloats(globalFloatChangedMessage.k, globalFloatChangedMessage.v);
                        //         OnGlobalFloatChanged?.Invoke(globalFloatChangedMessage.k);
                        //         break;
                        // 
                        //     case NetworkMessageType.GlobalFloatsRecapMessage:
                        //         GlobalFloatsRecapMessage globalFloatsRecapMessage = UnpackMessage<GlobalFloatsRecapMessage>(rawMessage);
                        //         _globalFloats = globalFloatsRecapMessage.k.Zip(globalFloatsRecapMessage.v, (s, i) => new { s, i }).ToDictionary(item => item.s, item => item.i);
                        //         foreach (var item in _globalFloats)
                        //         {
                        //             OnGlobalFloatChanged?.Invoke(item.Key);
                        //         }
                        //         OnGlobalFloatsReceived?.Invoke();
                        //         break;
                        // 
                        //     case NetworkMessageType.GlobalBoolChangedMessage:
                        //         GlobalBoolChangedMessage globalBoolChangedMessage = UnpackMessage<GlobalBoolChangedMessage>(rawMessage);
                        //         SetLocalGlobalBools(globalBoolChangedMessage.k, globalBoolChangedMessage.v);
                        //         OnGlobalBoolChanged?.Invoke(globalBoolChangedMessage.k);
                        //         break;
                        // 
                        //     case NetworkMessageType.GlobalBoolsRecapMessage:
                        //         GlobalBoolsRecapMessage globalBoolsRecapMessage = UnpackMessage<GlobalBoolsRecapMessage>(rawMessage);
                        //         _globalBools = globalBoolsRecapMessage.k.Zip(globalBoolsRecapMessage.v, (s, i) => new { s, i }).ToDictionary(item => item.s, item => item.i);
                        //         foreach (var item in _globalBools)
                        //         {
                        //             OnGlobalBoolChanged?.Invoke(item.Key);
                        //         }
                        //         OnGlobalBoolsReceived?.Invoke();
                        //         break;
                        // 
                        //     case NetworkMessageType.GlobalStringChangedMessage:
                        //         GlobalStringChangedMessage globalStringChangedMessage = UnpackMessage<GlobalStringChangedMessage>(rawMessage);
                        //         SetLocalGlobalStrings(globalStringChangedMessage.k, globalStringChangedMessage.v);
                        //         OnGlobalStringChanged?.Invoke(globalStringChangedMessage.k);
                        //         break;
                        // 
                        //     case NetworkMessageType.GlobalStringsRecapMessage:
                        //         GlobalStringsRecapMessage globalStringsRecapMessage = UnpackMessage<GlobalStringsRecapMessage>(rawMessage);
                        //         _globalStrings = globalStringsRecapMessage.k.Zip(globalStringsRecapMessage.v, (s, i) => new { s, i }).ToDictionary(item => item.s, item => item.i);
                        //         foreach (var item in _globalStrings)
                        //         {
                        //             OnGlobalStringChanged?.Invoke(item.Key);
                        //         }
                        //         OnGlobalStringsReceived?.Invoke();
                        //         break;
                        // 
                        //     case NetworkMessageType.GlobalVector2ChangedMessage:
                        //         GlobalVector2ChangedMessage globalVector2ChangedMessage = UnpackMessage<GlobalVector2ChangedMessage>(rawMessage);
                        //         SetLocalGlobalVector2(globalVector2ChangedMessage.k, globalVector2ChangedMessage.v);
                        //         OnGlobalVector2Changed?.Invoke(globalVector2ChangedMessage.k);
                        //         break;
                        // 
                        //     case NetworkMessageType.GlobalVector2RecapMessage:
                        //         GlobalVector2RecapMessage globalVector2RecapMessage = UnpackMessage<GlobalVector2RecapMessage>(rawMessage);
                        //         _globalVector2 = globalVector2RecapMessage.k.Zip(globalVector2RecapMessage.v, (s, i) => new { s, i }).ToDictionary(item => item.s, item => item.i);
                        //         foreach (var key in _globalVector2.Keys)
                        //         {
                        //             OnGlobalVector2Changed?.Invoke(key);
                        //         }
                        //         OnGlobalVector2sReceived?.Invoke();
                        //         break;
                        // 
                        //     case NetworkMessageType.GlobalVector3ChangedMessage:
                        //         GlobalVector3ChangedMessage globalVector3ChangedMessage = UnpackMessage<GlobalVector3ChangedMessage>(rawMessage);
                        //         SetLocalGlobalVector3(globalVector3ChangedMessage.k, globalVector3ChangedMessage.v);
                        //         OnGlobalVector3Changed?.Invoke(globalVector3ChangedMessage.k);
                        //         break;
                        // 
                        //     case NetworkMessageType.GlobalVector3RecapMessage:
                        //         GlobalVector3RecapMessage globalVector3RecapMessage = UnpackMessage<GlobalVector3RecapMessage>(rawMessage);
                        //         _globalVector3 = globalVector3RecapMessage.k.Zip(globalVector3RecapMessage.v, (s, i) => new { s, i }).ToDictionary(item => item.s, item => item.i);
                        //         foreach (var key in _globalVector3.Keys)
                        //         {
                        //             OnGlobalVector3Changed?.Invoke(key);
                        //         }
                        //         OnGlobalVector3sReceived?.Invoke();
                        //         break;
                        // 
                        //     case NetworkMessageType.GlobalVector4ChangedMessage:
                        //         GlobalVector4ChangedMessage globalVector4ChangedMessage = UnpackMessage<GlobalVector4ChangedMessage>(rawMessage);
                        //         SetLocalGlobalVector4(globalVector4ChangedMessage.k, globalVector4ChangedMessage.v);
                        //         OnGlobalVector4Changed?.Invoke(globalVector4ChangedMessage.k);
                        //         break;
                        // 
                        //     case NetworkMessageType.GlobalVector4RecapMessage:
                        //         GlobalVector4RecapMessage globalVector4RecapMessage = UnpackMessage<GlobalVector4RecapMessage>(rawMessage);
                        //         _globalVector4 = globalVector4RecapMessage.k.Zip(globalVector4RecapMessage.v, (s, i) => new { s, i }).ToDictionary(item => item.s, item => item.i);
                        //         foreach (var key in _globalVector4.Keys)
                        //         {
                        //             OnGlobalVector4Changed?.Invoke(key);
                        //         }
                        //         OnGlobalVector4sReceived?.Invoke();
                        //         break;
                        // 
                        //     case NetworkMessageType.StringMessage:
                        //         StringMessage stringMessage = UnpackMessage<StringMessage>(rawMessage);
                        //         OnStringMessage?.Invoke(stringMessage);
                        //         break;
                        // 
                        //     case NetworkMessageType.StringArrayMessage:
                        //         StringArrayMessage stringArrayMessage = UnpackMessage<StringArrayMessage>(rawMessage);
                        //         OnStringArrayMessage?.Invoke(stringArrayMessage);
                        //         break;
                        // 
                        //     case NetworkMessageType.PoseMessage:
                        //         PoseMessage poseMessage = UnpackMessage<PoseMessage>(rawMessage);
                        //         OnPoseMessage?.Invoke(poseMessage);
                        //         break;
                        // 
                        //     case NetworkMessageType.PoseArrayMessage:
                        //         PoseArrayMessage poseArrayMessage = UnpackMessage<PoseArrayMessage>(rawMessage);
                        //         OnPoseArrayMessage?.Invoke(poseArrayMessage);
                        //         break;
                        // 
                        //     case NetworkMessageType.BoolMessage:
                        //         BoolMessage boolMessage = UnpackMessage<BoolMessage>(rawMessage);
                        //         OnBoolMessage?.Invoke(boolMessage);
                        //         break;
                        // 
                        //     case NetworkMessageType.BoolArrayMessage:
                        //         BoolArrayMessage booArraylMessage = UnpackMessage<BoolArrayMessage>(rawMessage);
                        //         OnBoolArrayMessage?.Invoke(booArraylMessage);
                        //         break;
                        // 
                        //     case NetworkMessageType.QuaternionMessage:
                        //         QuaternionMessage quaternionMessage = UnpackMessage<QuaternionMessage>(rawMessage);
                        //         OnQuaternionMessage?.Invoke(quaternionMessage);
                        //         break;
                        // 
                        //     case NetworkMessageType.QuaternionArrayMessage:
                        //         QuaternionArrayMessage quaternionArrayMessage = UnpackMessage<QuaternionArrayMessage>(rawMessage);
                        //         OnQuaternionArrayMessage?.Invoke(quaternionArrayMessage);
                        //         break;
                        // 
                        //     case NetworkMessageType.Vector2Message:
                        //         Vector2Message vector2Message = UnpackMessage<Vector2Message>(rawMessage);
                        //         OnVector2Message?.Invoke(vector2Message);
                        //         break;
                        // 
                        //     case NetworkMessageType.Vector2ArrayMessage:
                        //         Vector2ArrayMessage vector2ArrayMessage = UnpackMessage<Vector2ArrayMessage>(rawMessage);
                        //         OnVector2ArrayMessage?.Invoke(vector2ArrayMessage);
                        //         break;
                        // 
                        //     case NetworkMessageType.Vector3Message:
                        //         Vector3Message vector3Message = UnpackMessage<Vector3Message>(rawMessage);
                        //         OnVector3Message?.Invoke(vector3Message);
                        //         break;
                        // 
                        //     case NetworkMessageType.Vector3ArrayMessage:
                        //         Vector3ArrayMessage vector3ArrayMessage = UnpackMessage<Vector3ArrayMessage>(rawMessage);
                        //         OnVector3ArrayMessage?.Invoke(vector3ArrayMessage);
                        //         break;
                        // 
                        //     case NetworkMessageType.Vector4Message:
                        //         Vector4Message vector4Message = UnpackMessage<Vector4Message>(rawMessage);
                        //         OnVector4Message?.Invoke(vector4Message);
                        //         break;
                        // 
                        //     case NetworkMessageType.Vector4ArrayMessage:
                        //         Vector4ArrayMessage vector4ArrayMessage = UnpackMessage<Vector4ArrayMessage>(rawMessage);
                        //         OnVector4ArrayMessage?.Invoke(vector4ArrayMessage);
                        //         break;
                        // 
                        //     case NetworkMessageType.ColorMessage:
                        //         ColorMessage colorMessage = UnpackMessage<ColorMessage>(rawMessage);
                        //         OnColorMessage?.Invoke(colorMessage);
                        //         break;
                        // 
                        //     case NetworkMessageType.ColorArrayMessage:
                        //         ColorArrayMessage colorArrayMessage = UnpackMessage<ColorArrayMessage>(rawMessage);
                        //         OnColorArrayMessage?.Invoke(colorArrayMessage);
                        //         break;
                        // 
                        //     case NetworkMessageType.FloatMessage:
                        //         FloatMessage floatMessage = UnpackMessage<FloatMessage>(rawMessage);
                        //         OnFloatMessage?.Invoke(floatMessage);
                        //         break;
                        // 
                        //     case NetworkMessageType.FloatArrayMessage:
                        //         FloatArrayMessage floatArrayMessage = UnpackMessage<FloatArrayMessage>(rawMessage);
                        //         OnFloatArrayMessage?.Invoke(floatArrayMessage);
                        //         break;
                        // 
                        //     case NetworkMessageType.ByteArrayMessage:
                        //         ByteArrayMessage byteArrayMessage = UnpackMessage<ByteArrayMessage>(rawMessage);
                        //         OnByteArrayMessage?.Invoke(byteArrayMessage);
                        //         break;
                        // 
                        //     case NetworkMessageType.SpawnMessage:
                        //         SpawnMessage spawnMessage = UnpackMessage<SpawnMessage>(rawMessage);
                        // 
                        //         Vector3 relativeSpawnPosition = new Vector3((float)spawnMessage.px, (float)spawnMessage.py, (float)spawnMessage.pz);
                        //         Quaternion relativeSpawnRotation = new Quaternion((float)spawnMessage.rx, (float)spawnMessage.ry, (float)spawnMessage.rz, (float)spawnMessage.rw);
                        //         Vector3 spawnScale = new Vector3((float)spawnMessage.sx, (float)spawnMessage.sy, (float)spawnMessage.sz);
                        // 
                        //         TransmissionObject spawnObject = PerformSpawn(spawnMessage.rf, false, spawnMessage.f, spawnMessage.i, relativeSpawnPosition, relativeSpawnRotation, spawnScale);
                        //         break;
                        // 
                        //     case NetworkMessageType.DespawnMessage:
                        //         DespawnMessage despawnMessage = UnpackMessage<DespawnMessage>(rawMessage);
                        // 
                        //         TransmissionObject despawnTarget = TransmissionObject.Get(despawnMessage.ig);
                        //         if (despawnTarget != null)
                        //         {
                        //             Destroy(despawnTarget.gameObject);
                        //         }
                        // 
                        //         break;
                        // 
                        //     case NetworkMessageType.SpawnRecapMessage:
                        //         SpawnRecapMessage spawnRecapMessage = UnpackMessage<SpawnRecapMessage>(rawMessage);
                        // 
                        //         Vector3 spawnRecapPosition = new Vector3((float)spawnRecapMessage.px, (float)spawnRecapMessage.py, (float)spawnRecapMessage.pz);
                        //         Quaternion spawnRecapRotation = new Quaternion((float)spawnRecapMessage.rx, (float)spawnRecapMessage.ry, (float)spawnRecapMessage.rz, (float)spawnRecapMessage.rw);
                        //         Vector3 spawnRecapScale = new Vector3((float)spawnRecapMessage.sx, (float)spawnRecapMessage.sy, (float)spawnRecapMessage.sz);
                        // 
                        //         TransmissionObject spawnRecapObject = PerformSpawn(spawnRecapMessage.rf, false, spawnRecapMessage.f, spawnRecapMessage.i, spawnRecapPosition, spawnRecapRotation, spawnRecapScale);
                        //         break;
                        // 
                        //     case NetworkMessageType.TransformSyncMessage:
                        //         TransformSyncMessage transformSyncMessage = UnpackMessage<TransformSyncMessage>(rawMessage);
                        //         OnTransformSync?.Invoke(transformSyncMessage);
                        //         break;
                        // 
                        //     case NetworkMessageType.OwnershipTransferenceRequestMessage:
                        //         OwnershipTransferenceRequestMessage ownershipTransferenceRequestMessage = UnpackMessage<OwnershipTransferenceRequestMessage>(rawMessage);
                        //         TransmissionObject target = TransmissionObject.Get(ownershipTransferenceRequestMessage.ig);
                        //         if (!target.ownershipLocked)
                        //         {
                        //             target.IsMine = false;
                        //             OnOwnershipLost?.Invoke(target);
                        //             Send(new OwnershipTransferenceGrantedMessage(target.guid, ownershipTransferenceRequestMessage.f));
                        //         }
                        //         else
                        //         {
                        //             Send(new OwnershipTransferenceDeniedMessage(target.guid, ownershipTransferenceRequestMessage.f));
                        //         }
                        //         break;
                        // 
                        //     case NetworkMessageType.OwnershipTransferenceDeniedMessage:
                        //         OwnershipTransferenceDeniedMessage ownershipTransferenceDeniedMessage = UnpackMessage<OwnershipTransferenceDeniedMessage>(rawMessage);
                        //         TransmissionObject denied = TransmissionObject.Get(ownershipTransferenceDeniedMessage.ig);
                        //         OnOwnershipTransferDenied?.Invoke(denied);
                        //         break;
                        // 
                        //     case NetworkMessageType.OwnershipTransferenceGrantedMessage:
                        //         OwnershipTransferenceGrantedMessage ownershipTransferenceGrantedMessage = UnpackMessage<OwnershipTransferenceGrantedMessage>(rawMessage);
                        //         TransmissionObject gained = TransmissionObject.Get(ownershipTransferenceGrantedMessage.ig);
                        //         OnOwnershipGained?.Invoke(gained);
                        //         break;
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
            foreach (var item in _peers)
            {
                if (Time.realtimeSinceStartup - item.Value > StalePeerTimeout)
                {
                    stalePeers.Add(item.Key);
                    if (!instance._peekPeers.Contains(item.Key)) instance._peekPeers.Add(item.Key);
                }
            }

            //stale peer removal:
            foreach (var item in stalePeers)
            {
                // Remove(item);
                _peers.Remove(item);
                //remove from ages by value:
                var deadAge = _peerAges.First(kvp => kvp.Value == item);
                _peerAges.Remove(deadAge.Key);
                // instance.OnPeerLost?.Invoke(item);
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

#if UNITY_DEBUG
            UdpClient receiveClient = new UdpClient(instance.receivePort);
            _receiveEndPoint = new IPEndPoint(IPAddress.Any, instance.sendPort);
            //catalog message:
            byte[] bytes = receiveClient.Receive(ref _receiveEndPoint);
            // byte[] bytes = _udpClient.Receive(ref _receiveEndPoint);

#else
            //catalog message:
            byte[] bytes = _udpClient.Receive(ref _receiveEndPoint);

#endif

            //get raw message for key evaluation:
            string serialized = Encoding.UTF8.GetString(bytes);

            Debug.Log(serialized);
            NetworkMessage rawMessage = JsonUtility.FromJson<NetworkMessage>(serialized);

            //address evaluation:
            if (rawMessage.f != MyNetworkUtilities.MyAddress)
            {
                //keys evaluations:
                if (rawMessage.a == instance.appKey && rawMessage.p == instance.privateKey)
                {
                    //we send the serialized string for easier debug messages:
                    _receivedMessages.Add(serialized);
                }
            }
        }
    }
}
