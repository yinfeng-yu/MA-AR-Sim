using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeSelect : MonoBehaviour
{
    public GameObject selectedObject;
    public Vector3 gazeDirection;
    public Vector3 gazePoint;
    public GameObject gazeCursorPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RegisterCurrentGazeTarget();
        gazeDirection = CoreServices.InputSystem.GazeProvider.GazeDirection;
        
        // CoreServices.InputSystem.GazeProvider.GazeCursorPrefab = gazeCursorPrefab;

        if (Input.GetMouseButtonDown(0) && selectedObject != null)
        {
            if (GetComponent<GazeButton>() != null)
            {
                // selectedObject.GetComponent<PressableButton>().PressDistance = selectedObject.GetComponent<PressableButton>().MaxPushDistance;
                Debug.Log("Press");
                selectedObject.GetComponent<GazeButton>().Press();
            }
        }

        if (RemoteInput.instance.confirm && selectedObject != null)
        {
            if (selectedObject.GetComponent<GazeButton>() != null)
            {
                // selectedObject.GetComponent<PressableButton>().PressDistance = selectedObject.GetComponent<PressableButton>().MaxPushDistance;
                Debug.Log("Remote Press");
                selectedObject.GetComponent<GazeButton>().Press();
            }
            
        }

        // int uiLayer = 5;
        // int layerMask = 1 << uiLayer;
        // 
        // Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        // RaycastHit hit;
        // if (Physics.Raycast(ray, out hit, 100f, layerMask))
        // {
        //     // RoomVertexHandle hoveredVertexHandle = hit.collider.gameObject.GetComponent<RoomVertexHandle>();
        //     // selectedVertexHandle = hoveredVertexHandle == null ? selectedVertexHandle : hoveredVertexHandle;
        //     
        //     if (hit.transform.gameObject.GetComponent<ARUIInterface>() != null)
        //     {
        //         if (selectedObject == null)
        //         {
        //             // selectedObject = hit.transform.gameObject;
        //             // Debug.Log($"Select: " + selectedObject.name);
        //             // selectedObject.GetComponent<ARUIInterface>().OnSelected();
        //         }
        //         else if (selectedObject != null && selectedObject != hit.transform.gameObject)
        //         {
        //             // Debug.Log($"Deselect: " + selectedObject.name);
        //             // selectedObject.GetComponent<ARUIInterface>().OnDeselected();
        //             // Debug.Log($"Select: " + selectedObject.name);
        //             // selectedObject = hit.transform.gameObject;
        //             // selectedObject.GetComponent<ARUIInterface>().OnSelected();
        //         }
        //     }
        // }
        // else
        // {
        //     if (selectedObject != null)
        //     {
        //         // Debug.Log($"Deselect: " + selectedObject.name);
        //         // selectedObject.GetComponent<ARUIInterface>().OnDeselected();
        //         // selectedObject = null;
        //     }
        // }
    }



    void RegisterCurrentGazeTarget()
    {
        if (CoreServices.InputSystem.GazeProvider.GazeTarget)
        {
            // Debug.Log("User gaze is currently over game object: "
            //     + CoreServices.InputSystem.GazeProvider.GazeTarget);
            selectedObject = CoreServices.InputSystem.GazeProvider.GazeTarget;
            gazePoint = CoreServices.InputSystem.GazeProvider.HitPosition;
        }
        else
        {
            selectedObject = null;
        }
    }
}
