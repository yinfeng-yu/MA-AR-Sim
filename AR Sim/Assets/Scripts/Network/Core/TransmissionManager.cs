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

public struct StreamDataHeader
{
    public int width;
    public int height;
}

[RequireComponent(typeof(NetworkMessageHandler))]
public class TransmissionManager : MonoBehaviour
{
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

    // [Tooltip("All GameObjects in this list (in addition to the Transmission GameObject) will receive SendMessages when RPC messages are sent.")]
    // public GameObject[] rpcTargets;

    public Pose globalPose;

    public bool debugOutgoing;
    public bool debugIncoming;

    public static DateTime startUpTime;

    public PlatformEnum platform;


    // Move Request
    public Action<string> moveRequestReceived;

    // Singleton
    public static TransmissionManager instance;

    //Private Variables:
    [SerializeField] private const float _heartbeatInterval = 2;
    private const float _reliableResendInterval = .5f;
    private const float _maxResendDuration = 7;
    private const float _stalePeerTimeout = 8;

    private static bool _receiveThreadAlive;
    private static ConcurrentBag<string> _receivedMessages = new ConcurrentBag<string>(); //do we need to be concerned about the constant growth of this?
    

    // private static Dictionary<string, float> _peers = new Dictionary<string, float>();
    private Dictionary<string, Peer> _peers = new Dictionary<string, Peer>();

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

    private static bool _quitting;
    private static long _age;

    //Init:
    private void Awake()
    {
        if (instance != this)
        {
            instance = this;
        }

        _age = DateTime.Now.Ticks;
        Initialize();
    }

    private void Reset()
    {
        appKey = NetworkUtilities.UniqueID();
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

        //fire off an awake event:
        Send(new NetworkMessage(NetworkMessageType.AwakeMessage, NetworkAudience.NetworkBroadcast, "", true, _age.ToString()));

        instance.StartCoroutine(Heartbeat());

    }


    /// <summary>
    /// Checks if a known peer still has an active heartbeat.
    /// </summary>
    public bool PeerAlive(string address)
    {
        // TODO
        return _peers.ContainsKey(address);
    }

    public List<Peer> GetPeersList()
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
    public void Send(NetworkMessage message)
    {
        // Generate transmission:
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

        // Send:
        if (string.IsNullOrEmpty(message.t))
        {
            //send to all peers:
            foreach (var item in instance.GetPeers())
            {
                endPoint.Address = IPAddress.Parse(item.Value.ipAddress);
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

    public async void SendAsync(NetworkMessage message)
    {
        //generate transmission:
        string serialized = JsonUtility.ToJson(message);
        byte[] bytes = Encoding.UTF8.GetBytes(serialized);

#if UNITY_DEBUG
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, instance.sendPort);
        // _udpClient = new UdpClient(instance.receivePort);
#else
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, instance.port);
#endif
        //send:
        if (string.IsNullOrEmpty(message.t))
        {
            //send to all peers:
            foreach (var item in instance.GetPeers())
            {
                endPoint.Address = IPAddress.Parse(item.Value.ipAddress);
                await _udpClient.SendAsync(bytes, bytes.Length, endPoint);
                
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
            await _udpClient.SendAsync(bytes, bytes.Length, endPoint);

            //debug:
            if (instance.debugOutgoing)
            {
                Debug.Log($"Sent {serialized} to {endPoint}");
            }
        }

        
    }

    private void ReceiveMessages()
    {
        while (_receivedMessages.Count > 0)
        {
            string rawMessage;
            if (_receivedMessages.TryTake(out rawMessage))
            {
                GetComponent<NetworkMessageHandler>().ProcessRawMessage(rawMessage);
            }
        }
    }

    private IEnumerator Heartbeat()
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
            yield return new WaitForSeconds(_heartbeatInterval);
            yield return null;
        }
    }

    //Threads:
    private void Receive()
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

    public Dictionary<string, Peer> GetPeers() { return _peers; }


    public void SendMoveRequest(SiteEnum a_siteEnum)
    {
        Send(new MoveRequestMessage(a_siteEnum.ToString()));
        Debug.Log("Move request sent");
    }

    public void SendYieldControlMessage(PlatformEnum a_platform)
    {
        Send(new YieldControlMessage(a_platform));
    }

    public void SendStreamAsync(byte[] a_data, int a_width, int a_height)
    {
        StreamDataHeader streamDataHeader;
        streamDataHeader.width = a_width;
        streamDataHeader.height = a_height;

        SendAsync(new StreamMessage(streamDataHeader, a_data));
    }


}
