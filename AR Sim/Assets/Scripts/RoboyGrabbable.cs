using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;
using OManipulator = Microsoft.MixedReality.Toolkit.UI.ObjectManipulator;

public class RoboyGrabbable : MonoBehaviour
{
    public bool leftHand;
    public bool rightHand;
    
    public GameObject grabPoint;
    private RoboyMovement roboy;
    private OManipulator manipulator;
    private BoundsControl boundsControl;
    private Rigidbody rb;
 
    private bool isFirstPOV;

    private void IsFirstPOV(object sender, bool e)
    {
        isFirstPOV = e;
        manipulator.enabled = e;
    }
    
    private void Start()
    {
        manipulator = GetComponentInChildren<OManipulator>();
        roboy = FindObjectOfType<RoboyMovement>();
        roboy.ChangePOV += IsFirstPOV;
        boundsControl = GetComponentInChildren<BoundsControl>();
        rb = GetComponentInChildren<Rigidbody>();
       
    }
    
    private void Grab()
    {
        if (rb != null)
        {
           rb.isKinematic = true;
           rb.detectCollisions = false;
        }
            
        if (leftHand) roboy.handControl.LeftGrab = grabPoint;
        if (rightHand) roboy.handControl.RightGrab = grabPoint;
        
    }

    private void LetGo(object sender, bool e)
    {
        if (e == false)
        {
            Debug.Log("Let Go");
        
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.detectCollisions = true;
            }
            if (leftHand) roboy.handControl.LeftGrab = null;
            if (rightHand) roboy.handControl.RightGrab = null;

            rightHand = leftHand = false;
        }
    }
    
    public void LetGo(bool handsClosed)
    {
        if (handsClosed == false)
        {
            Debug.Log("Let Go");
        
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.detectCollisions = true;
            }
            if (leftHand) roboy.handControl.LeftGrab = null;
            if (rightHand) roboy.handControl.RightGrab = null;

            rightHand = leftHand = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("stay");
        if ( !isFirstPOV)
        {
            boundsControl.Active = true;
            
            leftHand = other.gameObject.CompareTag("Left");
            rightHand = other.gameObject.CompareTag("Right");
            
            if (leftHand || rightHand)
            {
                Grab();
            }
        }
      
    }
    
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("exit");
        if (other.gameObject.CompareTag("Left"))
        {
            boundsControl.Active = false;
        }

    }
    
}
