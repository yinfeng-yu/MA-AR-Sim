using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;
using UnityEngine;

public class PointOfView : MonoBehaviour
{

    public Vector3 offsetThirdPOV; //0, 0.2, 0.5
    private Transform camera;

    public void Start()
    {
        camera = FindObjectOfType<Camera>().transform;
        transform.parent = camera;
    }

    public void TogglePOV()
    {
        if (transform.parent == null)
        {
            FirstPersonPOV();
        }
        else
        {
            ThirdPersonPOV();
        }
    }

    public void FirstPersonPOV()
    {
        MixedRealityPlayspace.Transform.position = this.transform.position;
        this.transform.parent = camera;
    }

    public void ThirdPersonPOV()
    {
        this.transform.parent = null;
        MixedRealityPlayspace.Transform.Translate(offsetThirdPOV);
    }
    
    public void ChangePOV(Vector3 offset)
    {
        MixedRealityPlayspace.Transform.Translate(offset);
    }
    
}
    
