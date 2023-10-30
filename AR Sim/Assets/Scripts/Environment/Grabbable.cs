using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    public bool isInRangeLeft = false;
    public bool isInRangeRight = false;

    public float range = 0.2f;

    public Transform scene;

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(HandController.Instance.leftHandIKTarget.position, transform.position) <= range)
        {
            isInRangeLeft = true;
        }
        else
        {
            isInRangeLeft = false;
        }

        if (Vector3.Distance(HandController.Instance.rightHandIKTarget.position, transform.position) <= range)
        {
            isInRangeRight = true;
        }
        else
        {
            isInRangeRight = false;
        }

        if (FingerController.Instance.isLeftGrab && isInRangeLeft)
        {
            Debug.Log("left grab");
            // transform.position = HandIK.Instance.leftGripPivot.position;
            transform.SetParent(FingerController.Instance.leftGripPivot);
            // GetComponent<Rigidbody>().useGravity = false;
            // GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

        else if (FingerController.Instance.isRightGrab && isInRangeRight)
        {
            Debug.Log("right grab");
            // transform.position = HandIK.Instance.rightGripPivot.position;
            transform.SetParent(FingerController.Instance.rightGripPivot);

            // GetComponent<Rigidbody>().useGravity = false;
            // GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

        else
        {

            // GetComponent<Rigidbody>().useGravity = true;
            transform.SetParent(scene);
        }
    }


}
