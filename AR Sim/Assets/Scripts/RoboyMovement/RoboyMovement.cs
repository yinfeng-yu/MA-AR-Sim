using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class RoboyMovement : MonoBehaviour
{
    public RoboyHands handControl;

    [SerializeField] Transform cameraPivot;

    #region event handlers
    public event EventHandler<bool> ChangePOV;
    public event EventHandler<bool> ToggleHandMovement;
    
    #endregion
    
    #region private variables
    
    [SerializeField] private Vector3 povDistance;
    [SerializeField] private GameObject sceneContent;
    private bool isFirstPersonView;
    private bool allowHandMovement;
    private HorizontalOrbitalSolver horizontalOrbitalSolver;
    private Transform parent;
    // private Transform camera;
    
    #endregion
    
    public virtual void OnAllowHandMovement()
    {
        OnAllowHandMovement(!allowHandMovement);
    }
    
    public virtual void OnAllowHandMovement(bool allow)
    {
        ToggleHandMovement?.Invoke(this, allow);
        allowHandMovement = allow;
        handControl.enabled = allow;
    }

    public virtual void OnChangePOV()
    {
        // OnChangePOV(!isFirstPersonView);
    }
    
    public virtual void OnChangePOV(bool isfirstPOV)
    {
        
        ChangePOV?.Invoke(this, isfirstPOV);
        this.isFirstPersonView = isfirstPOV;

        if (!isfirstPOV)
        {
            horizontalOrbitalSolver.LocalOffset += new Vector3(0, 0, povDistance.z);
            sceneContent.transform.position += MixedRealityPlayspace.Transform.forward * povDistance.z;
            handControl.HandOffset = new Vector3(0, 0, povDistance.z);
            
        }
        else
        {
            horizontalOrbitalSolver.LocalOffset = new Vector3(horizontalOrbitalSolver.LocalOffset.x, horizontalOrbitalSolver.LocalOffset.y, 0);
            sceneContent.transform.position -= MixedRealityPlayspace.Transform.forward * povDistance.z;
            handControl.HandOffset = Vector3.zero;
        }
        
    }

    private void Start()
    {
        horizontalOrbitalSolver = GetComponent<HorizontalOrbitalSolver>();
        handControl = GetComponent<RoboyHands>();
        sceneContent = GameObject.Find("+ SceneContent");
        // OnChangePOV(true);
        // OnAllowHandMovement(true);
        parent = transform.parent;
        transform.parent = null;
        // camera = Camera.main.transform;
    }

    private void Update()
    {
        if (!isFirstPersonView)
        {
            // parent.rotation = YRotation(parent.rotation, Quaternion.Inverse(Camera.main.transform.rotation));
            // cameraPivot.rotation = YRotation(parent.rotation, Quaternion.Inverse(Camera.main.transform.rotation));
        }
        // transform.rotation = YRotation(transform.rotation, camera.rotation);
        // transform.rotation = YRotation(transform.rotation, Camera.main.transform.rotation);
        // cameraPivot.forward = new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z);
        // cameraPivot.position = new Vector3(Camera.main.transform.position.x, 0f, Camera.main.transform.position.z);
        // cameraPivot.localPosition = new Vector3(cameraPivot.localPosition.x, 0f, cameraPivot.localPosition.z);
    }

    private Quaternion YRotation(Quaternion oldRotation, Quaternion rotation)
    {
        return Quaternion.Euler(oldRotation.eulerAngles.x ,rotation.eulerAngles.y, oldRotation.eulerAngles.z);
    }
    

}
