using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboyComponentsAccess : MonoBehaviour
{
    public RoboyHands roboyHands;
    public RoboyMovement roboyMovement;

    public static RoboyComponentsAccess instance;

    private void Awake()
    {
        if (instance != this)
        {
            instance = this;
        }
    }

}
