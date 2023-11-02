using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeSelector : Singleton<GazeSelector>
{
    public GameObject selectedObject;
    public Vector3 gazeDirection;
    public Vector3 gazePoint;

    public string[] selectableLayers = new[] { "MainMenuUI", "SceneUI" };
    // Update is called once per frame
    void Update()
    {
        RegisterCurrentGazeTarget();
        
        gazeDirection = CoreServices.InputSystem.GazeProvider.GazeDirection;
        
        // CoreServices.InputSystem.GazeProvider.GazeCursorPrefab = gazeCursorPrefab;

        if (Input.GetMouseButtonDown(0) && selectedObject != null)
        {
            if (selectedObject.GetComponent<GazeButton>() != null)
            {
                // selectedObject.GetComponent<PressableButton>().PressDistance = selectedObject.GetComponent<PressableButton>().MaxPushDistance;
                Debug.Log("Press");
                selectedObject.GetComponent<GazeButton>().Press();
            }
        }

        if (RemoteInput.GetConfirm() && selectedObject != null)
        {
            if (selectedObject.GetComponent<GazeButton>() != null)
            {
                RemoteInput.ConsumeConfirm();
                // selectedObject.GetComponent<PressableButton>().PressDistance = selectedObject.GetComponent<PressableButton>().MaxPushDistance;
                Debug.Log("Remote Press");
                selectedObject.GetComponent<GazeButton>().Press();
            }
            
        }

    }

    void RegisterCurrentGazeTarget()
    {
        var gazeDirection = CoreServices.InputSystem.GazeProvider.GazeDirection;

        int layerMask = 0;
        foreach (string layer in selectableLayers)
        {
            layerMask += 1 << LayerMask.NameToLayer(layer);
        }
        // string layer = "MainMenuUI";
        // int layerMask = 1 << LayerMask.NameToLayer(layer);
        // 
        // layer = "SceneUI";
        // layerMask += 1 << LayerMask.NameToLayer(layer);

        GameObject newSelected;

        // Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        Ray ray = new Ray(Camera.main.transform.position, gazeDirection);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, layerMask))
        {
            newSelected = hit.transform.gameObject;
            gazePoint = hit.point;

            // If something was selected
            if (selectedObject != null)
            {
                // If the old selected object is a GazeButton and is different than the newly selected, call EndGaze() of the old button
                if (selectedObject.GetComponent<GazeButton>() != null && selectedObject != newSelected)
                {
                    GazeButton button = selectedObject.GetComponent<GazeButton>();
                    button.EndGaze();
                }
            }

            // If the newly selected is a GazeButton
            if (newSelected.GetComponent<GazeButton>() != null)
            {
                GazeButton button = newSelected.GetComponent<GazeButton>();
                // If the GazeButton is not yet gazed, call StartGaze()
                if (button.isGazeTriggerable && !button.isGazed)
                {
                    button.StartGaze();
                }
            }

            selectedObject = newSelected;
        }
        else
        {
            if (selectedObject != null)
            {
                if (selectedObject.GetComponent<GazeButton>() != null)
                {
                    GazeButton button = selectedObject.GetComponent<GazeButton>();
                    button.EndGaze();
                }
            }

            selectedObject = null;
        }


        // if (CoreServices.InputSystem.GazeProvider.GazeTarget)
        // {
        //     // Debug.Log("User gaze is currently over game object: "
        //     //     + CoreServices.InputSystem.GazeProvider.GazeTarget);
        // 
        //     var gazedObject = CoreServices.InputSystem.GazeProvider.GazeTarget;
        //     if (LayerMask.LayerToName(gazedObject.layer) == "Scene")
        //     {
        //         selectedObject = gazedObject;
        //     }
        //     gazePoint = CoreServices.InputSystem.GazeProvider.HitPosition;
        // }
        // else
        // {
        //     selectedObject = null;
        // }
    }
}
