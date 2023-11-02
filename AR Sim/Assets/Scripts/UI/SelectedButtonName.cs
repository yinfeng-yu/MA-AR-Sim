using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectedButtonName : MonoBehaviour
{
    public TextMeshPro textMeshPro;

    // Update is called once per frame
    void Update()
    {
        if (GazeSelector.Instance.selectedObject != null)
        {
            if (GazeSelector.Instance.selectedObject.GetComponent<GazeButton>() != null)
            {
                textMeshPro.text = GazeSelector.Instance.selectedObject.GetComponent<GazeButton>().buttonName;
            }
        }
    }
}
