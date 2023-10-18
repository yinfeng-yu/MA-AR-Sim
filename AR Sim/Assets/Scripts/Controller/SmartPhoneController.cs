using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartphoneController : Singleton<SmartphoneController>
{

    public GameObject smartphonePointer;
    public GameObject pointerLine;
    public Transform smartphonePivot;
    public GameObject calibTarget;

    private void Start()
    {
        HidePointer();
        HidePointerLine();
        HideCalibTarget();
    }

    public void SetPointerPosCalibMode()
    {
        smartphonePointer.transform.position = calibTarget.transform.position + 0.5f * Vector3.left;
    }

    public void ShowPointer()
    {
        smartphonePointer.SetActive(true);
    }

    public void HidePointer()
    {
        smartphonePointer.SetActive(false);
    }

    public void ShowPointerLine()
    {
        pointerLine.SetActive(true);
    }

    public void HidePointerLine()
    {
        pointerLine.SetActive(false);
    }

    public void ShowCalibTarget()
    {
        calibTarget.SetActive(true);
    }

    public void HideCalibTarget()
    {
        calibTarget.SetActive(false);
    }
}
