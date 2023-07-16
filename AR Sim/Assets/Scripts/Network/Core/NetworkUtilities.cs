// ---------------------------------------------------------------------
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/terms/developer
//
// ---------------------------------------------------------------------

using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public static class NetworkUtilities
{
    //Public Properties:
    public static string MyAddress
    {
        get
        {
            if (string.IsNullOrEmpty(_address))
            {
                string hostName = Dns.GetHostName();

                IPAddress[] ip = Dns.GetHostEntry(hostName).AddressList;

                foreach (var item in ip)
                {
                    if (item.AddressFamily == AddressFamily.InterNetwork)
                    {
                        _address = item.ToString();
                    }
                }

                if (string.IsNullOrEmpty(_address))
                {
                    _address = "0.0.0.0";
                }
            }

            return _address;
        }
    }

    public static string UniqueID()
    {
        string output = "";
        string guid = Guid.NewGuid().ToString();
        string[] segments = guid.Split('-');

        //translate each "chunk" of a guid into a single number:
        foreach (var segment in segments)
        {
            int part = 0;
            foreach (char piece in segment)
            {
                //change letters to numbers:
                if (Char.IsLetter(piece))
                {
                    part += (int)piece % 32;
                }
                else
                {
                    part += (int)part;
                }
            }

            //loop to keep range between 0-9:
            output += part % 10;
        }

        //pad a few extra random numbers to get us to 10:
        for (int i = 0; i < 5; i++)
        {
            output += Mathf.Round(UnityEngine.Random.value * 10) % 10;
        }

        return output;
    }

    public static T UnpackMessage<T>(string rawMessage)
    {
        return JsonUtility.FromJson<T>(rawMessage);
    }

    //Private Variables:
    private static string _address;
}