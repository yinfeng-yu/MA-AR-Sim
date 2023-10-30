using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobodyHands
{
    private static bool isIKEnabled = true;

    public static void EnableIK()
    {  
        isIKEnabled = true; 
    }

    public static void DisableIK() 
    {  
        isIKEnabled = false; 
    }

    public static bool IsIKEnabled => isIKEnabled;
    

}