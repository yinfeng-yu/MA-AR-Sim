using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ARUIInterface : MonoBehaviour
{
    public Material deselectedMaterial;
    public Material selectedMaterial;

    public void OnSelected()
    {
        foreach (Transform t in transform)
        {
            if (t.gameObject.GetComponent<Renderer>() != null && t.gameObject.GetComponent<TextMeshProUGUI>() == null)
            {
                t.gameObject.GetComponent<Renderer>().material = selectedMaterial;
            }
        }
    }

    public void OnDeselected()
    {
        foreach (Transform t in transform)
        {
            if (t.gameObject.GetComponent<Renderer>() != null && t.gameObject.GetComponent<TextMeshProUGUI>() == null)
            {
                t.gameObject.GetComponent<Renderer>().material = deselectedMaterial;
            }
        }
    }

}
