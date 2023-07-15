//ReadMe
//author:Pilot.Phil
//time:2022-04-23
//���ã���unity3D��camera�Ļ���ͨ��RenderTexture��ȡbytes�ķ�ʽ������UDPЭ�鷢�ͣ�ʹ����������ʹ����ͬЭ�����ͼƬ����
//����Ӧ��Ϊһ��camera��component
//
//�ο�1��https://blog.csdn.net/qq_34907362/article/details/119864856
//Unity Sockectʵ�ֻ���ʵʱ����
//
//�ο�2��https://cloud.tencent.com/developer/article/1681722
//C#�̳�֮C#��ʹ��UDPͨ��ʵ��
//
//�������TCPЭ�鷢�͵�
//����TCPЭ���UDPЭ����
//���ҽ�����ڱ����ڱ���ͨ��
//���Ժ���udp�ĵ�������
//��дΪudpģʽ
//��������ܣ�Ҳ���˴���

//python ����bytes��ͼƬ ���̣�
//import socket
//import cv2
//import numpy as np

//s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
//# �󶨶˿�:
//s.bind(('127.0.0.1', 8848))
//while True:
// # ��������:
// data = s.recv(50000)
// # ����
// nparr = np.fromstring(data, np.uint8)
// img_decode = cv2.imdecode(nparr, cv2.IMREAD_COLOR)
// cv2.imshow('result', img_decode)
// cv2.waitKey(10)


using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


public class CameraUdpStreamer : MonoBehaviour
{
    UdpClient client;
    IPEndPoint remotePoint = null;

    public Camera cam;
    RenderTexture renderTexture = null;
    Texture2D texture2D = null;

    /// <summary>
    /// IP��ַ
    /// </summary>
    public string IP = "127.0.0.1";

    /// <summary>
    /// �˿�
    /// </summary>
    public uint Port = 8848;

    /// <summary>
    /// ��Ƶ���������
    /// </summary>
    [Range(1, 100)]
    public int Quality = 50;

    /// <summary>
    /// ��ʾ���Դ�ӡ��Ϣ
    /// </summary>
    public bool _debug = false;

    /// <summary>
    /// ͼ���͵�Ƶ��
    /// </summary>
    [Range(1, 60)]
    public uint Freq = 15;

    private double interval;

    void Start()
    {
        //1.��ʼ��udp
        IPAddress remoteIP = IPAddress.Parse(IP);
        remotePoint = new IPEndPoint(remoteIP, (int)Port);
        client = new UdpClient();

        //2.�������س�ʼ��
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        renderTexture.enableRandomWrite = true;
        cam = this.GetComponent<Camera>();//��ȡ�����ϵ�������
        cam.targetTexture = renderTexture;
        texture2D = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        interval = 1.0 / Freq;
    }

    private void FixedUpdate()
    {
        //this.SendTextureAsync();
        if (interval > 0)
        {
            Debug.Log(interval);
            interval -= Time.deltaTime;
        }
        else
        {
            this.SendTextureAsync();
            interval = 1.0 / Freq;
        }
    }

    /// <summary>
    /// �첽��ͼ���ͷ���
    /// </summary>
    /// <returns></returns>
    private async Task SendTextureAsync()
    {
        // ��ȡ��Ļ���ؽ�����Ⱦ
        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        RenderTexture.active = null;

        //�����jpg
        byte[] bytes = texture2D.EncodeToJPG(Quality);

        await client.SendAsync(bytes, bytes.Length, remotePoint);

        if (_debug) Debug.Log(this.name + " SendTextureAsync OK");
    }

    /// <summary>
    /// �����˳�ʱ�ر�udp����
    /// </summary>
    private void OnApplicationQuit()
    {
        client?.Close();

        if (_debug) Debug.Log(this.name + " UDP client close");
    }
}