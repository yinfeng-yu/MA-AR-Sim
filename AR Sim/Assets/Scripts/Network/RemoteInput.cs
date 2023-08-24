using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteInput : MonoBehaviour
{
    public static RemoteInput instance;
    private void Awake()
    {
        if (instance != this) instance = this;
    }

    private bool _confirm;
    public bool confirm
    {
        get
        {
            bool temp = _confirm;
            _confirm = false;
            return temp;
        }
        
        set
        {
            _confirm = value;
        }
    }

}
