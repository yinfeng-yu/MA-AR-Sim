using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    Quaternion deviceOrientation;
    [SerializeField] public Transform calibTarget;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //GlobalVariableManager.Instance.GlobalQuaternions.TryGetValue("deviceOrientation", out deviceOrientation);
        //transform.localRotation = deviceOrientation * Quaternion.Inverse(Quaternion.Inverse(calibTarget.localRotation) * deviceOrientation);
        transform.localRotation = calibTarget.rotation;
    }
}
