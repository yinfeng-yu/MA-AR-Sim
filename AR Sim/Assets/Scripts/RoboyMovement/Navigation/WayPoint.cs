using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    private RoboyMovement roboy;

    private void Start()
    {
        roboy = FindObjectOfType<RoboyMovement>();
        if(roboy != null) roboy.ChangePOV += EnableSwitch;
    }

    private void EnableSwitch(object sender, bool e)
    {
        this.gameObject.SetActive(e);
    }

    public void MoveHere()
    {
        if(roboy == null) roboy =  FindObjectOfType<RoboyMovement>();
       
        Debug.Log("Move Here");
        
       // if(roboy != null) roboy.currentWayPoint = this;
    }
    
}
