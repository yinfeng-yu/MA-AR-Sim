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

[Serializable]
public class Peer
{
    public string key;
    public string ipAddress;
    public float age;

    public Peer(string _key, string _ipAddress, float _age)
    {
        key = _key;
        ipAddress = _ipAddress;
        age = _age;
    }
}

public class NetworkTransmitter : Transmitter
{
    //Public Variables:
    public int Port = 23000;

    // For Unity Editor debug use
    public int SendPort = 23001;
    public int ReceivePort = 23002;

    public int bufferSize = 2048;

    public bool debugOutgoing;
    public bool debugIncoming;

    private UdpClient _udpClient;
    private Dictionary<string, Peer> _peers = new Dictionary<string, Peer>();

    private static Thread _receiveThread;
    private static IPEndPoint _receiveEndPoint = new IPEndPoint(IPAddress.Any, 0);
    private static bool _receiveThreadAlive;

    private bool _initialized;

    private static ConcurrentBag<string> _receivedMessages = new ConcurrentBag<string>();

    private const float _heartbeatInterval = 2;
    private const float _stalePeerTimeout = 8;
    private static long _age;

    private static float _reliableResendInterval = .5f;
    private static float _maxResendDuration = 7;
    public List<string> _confirmedReliableMessages = new List<string>();
    private static Dictionary<string, NetworkMessage> _unconfirmedReliableMessages = new Dictionary<string, NetworkMessage>();

    public static string appKey;

    public Dictionary<string, Peer> peers
    {
        get => _peers;
    }

    public override void Initialize()
    {
        // flag initializtion complete:
        if (_initialized)
        {
            return;
        }
        _initialized = true;

        appKey = NetworkUtilities.UniqueID();

        //establish socket:
        bool socketOpen = false;
        while (!socketOpen)
        {
            try
            {
#if UNITY_EDITOR
                _udpClient = new UdpClient(ReceivePort);
#else
                udpClient = new UdpClient(Port);
#endif

                _udpClient.Client.SendBufferSize = bufferSize;
                _udpClient.Client.ReceiveBufferSize = bufferSize;
                socketOpen = true;
            }
            catch (Exception)
            {
            }
        }

        _age = DateTime.Now.Ticks;

        //establish receive thread:
        _receiveThreadAlive = true;
        _receiveThread = new Thread(new ThreadStart(Receive));
        _receiveThread.IsBackground = true;
        _receiveThread.Start();
        
        //fire off an awake event:
        Send(new NetworkMessage(NetworkMessageType.AwakeMessage, NetworkAudience.NetworkBroadcast, "", true, _age.ToString()));
        
        StartCoroutine(Heartbeat());
        StartCoroutine(ReliableRetry());
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


    public override void Receive()
    {
        while (_receiveThreadAlive)
        {
            byte[] bytes = _udpClient.Receive(ref _receiveEndPoint);

            //get raw message for key evaluation:
            string serialized = Encoding.UTF8.GetString(bytes);

            NetworkMessage rawMessage = JsonUtility.FromJson<NetworkMessage>(serialized);

            // keys evaluations:
            if (rawMessage.a != appKey)
            {
                //we send the serialized string for easier debug messages:
                _receivedMessages.Add(serialized);
            }

            // if (rawMessage.f != NetworkUtilities.MyAddress)
            // {
            //     _receivedMessages.Add(serialized);
            // }

        }
    }

    public override void ReceiveMessages()
    {
        while (_receivedMessages.Count > 0)
        {
            string rawMessage;
            if (_receivedMessages.TryTake(out rawMessage))
            {
                // get message:
                NetworkMessage currentMessage = JsonUtility.FromJson<NetworkMessage>(rawMessage);

                // debug:
                if (debugIncoming && currentMessage.f != NetworkUtilities.MyAddress)
                {
                    Debug.Log($"Received {rawMessage} from {currentMessage.f}");
                }
                
                // parse status:
                bool needToParse = true;

                // reliable message?
                if (currentMessage.r == 1)
                {
                    if (_confirmedReliableMessages.Contains(currentMessage.g))
                    {
                        // we have previously consumed this message but the confirmation failed so we only
                        // need to focus on sending another confirmation:
                        needToParse = false;
                        // continue;
                    }
                    else
                    {
                        // mark this reliable message as confirmed:
                        _confirmedReliableMessages.Add(currentMessage.g);
                    }
                
                    //send back confirmation message with same guid:
                    NetworkMessage confirmationMessage = new NetworkMessage(
                        NetworkMessageType.ConfirmedMessage,
                        NetworkAudience.NetworkBroadcast,
                        currentMessage.f,
                        false,
                        "",
                        currentMessage.g);
                
                    Send(confirmationMessage);
                }

                if (currentMessage.ty == (short) NetworkMessageType.ConfirmedMessage)
                {
                    // confirmed!
                    // Debug.Log("Sent Reliable Message Confirmed!");
                    _unconfirmedReliableMessages.Remove(currentMessage.g);
                    // OnSendMessageSuccess?.Invoke(currentMessage.g);
                    needToParse = false;
                }

                //parsing needed?
                if (!needToParse)
                {
                    continue;
                }

                NetworkMessageHandler.ProcessMessage(rawMessage, currentMessage, this);
                
                
            }
        }
    }

    public override void Send(Message message)
    {
        NetworkMessage networkMessage = (NetworkMessage) message;

        // reliable logging:
        if (networkMessage.r == 1)
        {
            if (!_unconfirmedReliableMessages.ContainsKey(networkMessage.g))
            {
                // // set target counts:
                // if (string.IsNullOrEmpty(networkMessage.t))
                // {
                //     networkMessage.ts = _peers.Count;
                // }
                // else
                // {
                //     networkMessage.ts = 1;
                // }

                _unconfirmedReliableMessages.Add(networkMessage.g, networkMessage);
            }
        }

        string serialized = message.ProduceString();
        byte[] data = message.ProduceBytes();

#if UNITY_EDITOR
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, SendPort);
        // _udpClient = new UdpClient(instance.receivePort);
#else
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, Port);
#endif

        // Size check:
        // if (data.Length > udpClient.Client.SendBufferSize)
        // {
        //     Debug.Log($"Message too large to send! Buffer is currently {BufferSize} bytes and you are tring to send {data.Length} bytes. Try increasing the buffer size.");
        //     return;
        // }

        // Send:
        if (string.IsNullOrEmpty(networkMessage.t))
        {
            //send to all peers:
            foreach (var item in peers)
            {
                endPoint.Address = IPAddress.Parse(item.Value.ipAddress);
                _udpClient.Send(data, data.Length, endPoint);
        
                // Debug:
                if (debugOutgoing)
                {
                    Debug.Log($"Sent {serialized} to {endPoint}");
                }
            }
        }
        else
        {
            endPoint.Address = IPAddress.Parse(networkMessage.t);
            _udpClient.Send(data, data.Length, endPoint);
        
            // Debug:
            if (debugOutgoing)
            {
                Debug.Log($"Sent {serialized} to {endPoint}");
            }
        }

    }

    private IEnumerator ReliableRetry()
    {
        while (true)
        {
            // iterate a copy so we don't have issues with inbound confirmations:
            foreach (var item in _unconfirmedReliableMessages.Values.ToArray())
            {
                Send(item);
            }

            //loop:
            yield return new WaitForSeconds(_reliableResendInterval);
            yield return null;
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
            foreach (var item in peers.ToList())
            {
                if (Time.realtimeSinceStartup - item.Value.age > _stalePeerTimeout)
                {
                    stalePeers.Add(item.Key);
                }
            }

            //stale peer removal:
            foreach (var item in stalePeers)
            {
                peers.Remove(item);
            }

            //loop:
            yield return new WaitForSeconds(_heartbeatInterval);
            yield return null;
        }
    }


}
