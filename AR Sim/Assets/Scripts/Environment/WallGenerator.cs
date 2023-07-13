using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : MonoBehaviour
{
    [SerializeField] int _width;
    [SerializeField] int _height;
    [SerializeField] float _spacing = 3.5f; // Default
    [SerializeField] public bool editMode = false;

    [SerializeField] Transform _floorParent;
    [SerializeField] GameObject _wallPrefab;

    public enum WallType
    {

    }

    [ExecuteInEditMode]
    void Update()
    {
        if (editMode)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.tag == "Floor")
                {
                    if (Input.GetMouseButtonDown(0))
                    {

                    }
                }
                // selectedVertexHandle = hit.collider.gameObject.GetComponent<RoomVertexHandle>();
                // 
                // if (selectedVertexHandle != null)
                // {
                //     if (Input.GetMouseButtonDown(0))
                //     {
                //         m_selected = true;
                // 
                //         if (m_oldSelectedVertexHandle != null)
                //         {
                //             m_oldSelectedVertexHandle.Deselect();
                //         }
                // 
                //         selectedVertexHandle.Select();
                //         m_oldSelectedVertexHandle = selectedVertexHandle;
                //     }
                // }
            }
        }
    }

    public void SwitchEditMode()
    {
        editMode = !editMode;
    }
}
