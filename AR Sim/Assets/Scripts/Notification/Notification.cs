using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public enum TaskType
{
    Displace,
    Patrol,
}

public enum TaskStatus
{
    None,
    Start,
    End,
    Unknown,
}

[Serializable]
public struct TimeStamp
{
    public int hour;
    public int minute;
    public int second;
}

public class Notification
{
    public TaskType taskType;
    public TaskStatus taskStatus;
    public string data;
    // public string localRefKey;
    public TimeStamp timeStamp;

    public Notification(TaskType a_taskType, TaskStatus a_taskStatus, string a_data, string a_localRefKey, TimeStamp a_timeStamp)
    {
        taskType = a_taskType;
        taskStatus = a_taskStatus;
        data = a_data;
        // localRefKey = a_localRefKey;
        timeStamp = a_timeStamp;
    }

    public string ProduceContent()
    {
        switch (taskType)
        {
            case TaskType.Displace:
                switch (taskStatus)
                {
                    case (TaskStatus.End):
                        return $"Roboy has arrived at position {data}";
                    default:
                        break;
                }
                return "";
            case TaskType.Patrol:
                switch (taskStatus)
                {
                    case (TaskStatus.End):
                        return $"Roboy has stopped the partrol.";
                    default:
                        break;
                }
                return "";
            default:
                return "";
        }
    }

}