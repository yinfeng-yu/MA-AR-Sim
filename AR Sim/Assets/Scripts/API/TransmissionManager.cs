using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Platform
{
    AR,
    Smartphone,
    Robody,
}

public class TransmissionManager : Singleton<TransmissionManager>
{
    /// <summary>
    /// The message transmitter using UDP over LAN; used for communicating with AR Headset
    /// </summary>
    public NetworkTransmitter NetworkTransmitter;

    // public SomeNewTransmitter SomeNewTransmitter;

    public void SendTo(Message message, Platform platform)
    {
        switch (platform)
        {
            case Platform.AR:
                NetworkTransmitter.Send(message);
                break;
            case Platform.Smartphone:
                NetworkTransmitter.Send(message);
                break;
            case Platform.Robody:
                Debug.Log("Send data to Robody");
                // SomeNewTransmitter.Send(message)
                break;
            default:
                break;
        }
    }

}
