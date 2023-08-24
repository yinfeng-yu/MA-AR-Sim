using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartPhoneController : MonoBehaviour
{
    public static SmartPhoneController instance;
    private void Awake()
    {
        if (instance != this)
        {
            instance = this;
        }
    }

    public Transform smartphonePointer;
    public Transform pointerLine;
    public Transform smartphonePivot;

    private void Start()
    {
        TurnOffPointer();
        TurnOffPointerLine();
    }

    public void TurnOnPointer()
    {
        smartphonePointer.gameObject.SetActive(true);
    }

    public void TurnOffPointer()
    {
        smartphonePointer.gameObject.SetActive(false);
    }

    public void TurnOnPointerLine()
    {
        pointerLine.gameObject.SetActive(true);
    }

    public void TurnOffPointerLine()
    {
        pointerLine.gameObject.SetActive(false);
    }

    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.blue;
    //     Gizmos.DrawLine(_mobilePhonePointer.position, _mobilePhonePointer.position + _mobilePhonePointer.forward * 5f);
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawLine(_mobilePhonePointer.position, _mobilePhonePointer.position + _mobilePhonePointer.right * 5f);
    //     Gizmos.color = Color.green;
    //     Gizmos.DrawLine(_mobilePhonePointer.position, _mobilePhonePointer.position + _mobilePhonePointer.up * 5f);
    // 
    //     Gizmos.color = Color.yellow;
    //     Gizmos.DrawWireSphere(_mobilePhonePointer.position, _deviceRange);
    // }
}
