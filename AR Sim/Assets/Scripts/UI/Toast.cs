using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.WSA;

public class Toast : MonoBehaviour
{
    // public bool isShown = true;

    [SerializeField] private TextMeshProUGUI _toastText;


    // Update is called once per frame
    void Update()
    {
        Transform mainCameraTransform = Camera.main.transform;
        transform.position = mainCameraTransform.position + mainCameraTransform.forward * 0.5f;
        transform.LookAt(mainCameraTransform.position);
        transform.Rotate(0, 180, 0);

        // if (Input.GetMouseButton(0))
        // {
        //     if (isShown)
        //     {
        //         HideToast();
        //         isShown = false;
        //     }
        //     else
        //     {
        //         ShowToast();
        //         isShown = true;
        //     }
        // }
    }

    public void ShowToast()
    {
        foreach (Transform c in transform)
        {
            c.gameObject.SetActive(true);
        }
    }

    public void HideToast()
    {
        foreach (Transform c in transform)
        {
            c.gameObject.SetActive(false);
        }
    }

    public void SetToastText(string text)
    {
        _toastText.text = text;
    }
}
