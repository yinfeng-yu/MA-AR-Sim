/*
    -----------------------
    UDP-Send
    -----------------------
   
    // 127.0.0.1 : 5009
    // nc -lu 127.0.0.1 5009
*/
using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPSend : MonoBehaviour
{
    private static int localPort;
    private string IP;  // define in init
    public int port;  // define in init
    IPEndPoint remoteEndPoint;
    UdpClient client;
    string strMessage = "";

    private static void Main()
    {
        UDPSend sendObj = new UDPSend();
        sendObj.Init();
        sendObj.SendEndless(" endless infos \n");
    }

    // start from unity3d
    public void Start()
    {
        Init();
    }

    // OnGUI
    // void OnGUI()
    // {
    //     Rect rectObj = new Rect(40, 380, 200, 400);
    //     GUIStyle style = new GUIStyle();
    //     style.alignment = TextAnchor.UpperLeft;
    //     GUI.Box(rectObj, "# UDPSend-Data\n127.0.0.1 " + port + " #\n"
    //                 + "shell> nc -lu 127.0.0.1  " + port + " \n"
    //             , style);
    //     strMessage = GUI.TextField(new Rect(40, 420, 140, 20), strMessage);
    //     if (GUI.Button(new Rect(190, 420, 40, 20), "send"))
    //     {
    //         SendString(strMessage + "\n");
    //     }
    // }

    // init
    public void Init()
    {
        print("UDPSend.init()");
        IP = "127.0.0.1";
        // port = 5009;
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient();
        print("Sending to " + IP + " : " + port);
        print("Testing: nc -lu " + IP + " : " + port);

    }

    private void InputFromConsole()
    {
        try
        {
            string text;
            do
            {
                text = Console.ReadLine();
                if (text != "")
                {
                    byte[] data = Encoding.UTF8.GetBytes(text);
                    client.Send(data, data.Length, remoteEndPoint);
                }
            } while (text != "");
        }
        catch (Exception err)
        {
            print(err.ToString());
        }

    }

    // sendData
    public void SendString(string message)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            client.Send(data, data.Length, remoteEndPoint);
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }
    private void SendEndless(string testStr)
    {
        do
        {
            SendString(testStr);
        }
        while (true);
    }
}