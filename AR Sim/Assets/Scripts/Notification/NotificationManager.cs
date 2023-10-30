using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationManager : Singleton<NotificationManager>
{
    
    public void SendNotification(TaskType taskType, TaskStatus taskStatus, string data)
    {
        TimeStamp timeStamp;
        timeStamp.hour = System.DateTime.Now.Hour;
        timeStamp.minute = System.DateTime.Now.Minute;
        timeStamp.second = System.DateTime.Now.Second;

        Notification notification = new Notification(taskType, taskStatus, data, "", timeStamp);
        string serializedNotification = JsonUtility.ToJson(notification);
        TransmissionManager.Instance.SendTo(new NetworkMessage(NetworkMessageType.NotificationMessage, NetworkAudience.NetworkBroadcast, "", false, serializedNotification), Platform.Smartphone);
    }
}
