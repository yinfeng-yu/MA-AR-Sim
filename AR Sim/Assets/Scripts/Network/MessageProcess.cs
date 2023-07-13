using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// public enum UDPMessageType
// {
//     SE, // Site Enum
// 
//     UD, // Undefined
// 
// }

public class MessageProcess : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MyTransmission.instance.moveRequestReceived += ExecuteMove;
    }

    private void OnDestroy()
    {
        MyTransmission.instance.moveRequestReceived -= ExecuteMove;
    }

    // Update is called once per frame
    void Update()
    {
        // if (!GetComponent<UDPReceive>().packetProcessed)
        // {
        //     GetComponent<UDPReceive>().packetProcessed = true;
        // 
        //     string lastReceivedUDPPacket = GetComponent<UDPReceive>().lastReceivedUDPPacket;
        //     UDPMessageType messageType = GetMessageType(lastReceivedUDPPacket);
        //     switch (messageType)
        //     {
        //         case UDPMessageType.SE:
        //             string siteEnumStr = ProcessSiteEnum(lastReceivedUDPPacket);
        //             Transform targetTransform = SiteManager.instance.GetTransform(siteEnumStr);
        //             NavigationManager.instance.GoToTarget(targetTransform, siteEnumStr);
        //             break;
        //         default:
        //             break;
        //     }
        // }
    }

    void ExecuteMove(string siteEnumStr)
    {
        Debug.Log("Move request received");
        Transform targetTransform = SiteManager.instance.GetTransform(siteEnumStr);
        NavigationManager.instance.GoToTarget(targetTransform, siteEnumStr);
    }

    // UDPMessageType GetMessageType(string a_packet)
    // {
    //     if (a_packet.Substring(1, 2) == "SE") return UDPMessageType.SE;
    //     return UDPMessageType.UD;
    // }
    // 
    // string ProcessSiteEnum(string a_packet)
    // {
    //     return a_packet.Substring(5, a_packet.Length - 5);
    // }
}
