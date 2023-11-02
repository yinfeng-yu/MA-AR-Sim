using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    public static void ShowToast(string text, float timeout)
    {
        try
        {
            ToastManager.Instance.ShowToast(text, timeout);
        }
        catch (Exception e) { Debug.Log(e); }
        
    }
}
