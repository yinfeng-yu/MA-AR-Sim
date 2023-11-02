using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartphoneController : Singleton<SmartphoneController>
{

    public GameObject smartphonePointer;
    public GameObject pointerLine;
    public GameObject calibTarget;

    public Quaternion initOrientation;
    public Vector3 initPosition;

    private void Start()
    {
        HidePointer();
        HidePointerLine();
        HideCalibTarget();
    }

    public void SetPointerPosCalibMode()
    {
        smartphonePointer.transform.position = calibTarget.transform.position;
    }

    public void SetPointerPosPointerMode(Handedness handedness)
    {
        switch (handedness)
        {
            case Handedness.Left:
                // smartphonePointer.transform.position = Robody.Instance.position + Vector3.up * 2 + Vector3.left * 0.4f + Vector3.forward * 0.5f;
                smartphonePointer.transform.position = calibTarget.transform.position - transform.right * 0.3f - transform.forward * 0.1f;
                break;
            case Handedness.Right:
                // smartphonePointer.transform.position = Robody.Instance.position + Vector3.up * 2 + Vector3.right * 0.4f + Vector3.forward * 0.5f;
                smartphonePointer.transform.position = calibTarget.transform.position + transform.right * 0.3f - transform.forward * 0.1f;
                break;
            default:
                break;
        }
    }

    public void ResetPointerPos()
    {
        smartphonePointer.transform.position = calibTarget.transform.position;
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
