using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    public bool isInRangeLeft = false;
    public bool isInRangeRight = false;

    public float range = 0.2f;

    public Transform scene;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(HandIK.Instance.leftGripPivot.position, transform.position) <= range)
        {
            isInRangeLeft = true;
        }
        else
        {
            isInRangeLeft = false;
        }

        if (Vector3.Distance(HandIK.Instance.rightGripPivot.position, transform.position) <= range)
        {
            isInRangeRight = true;
        }
        else
        {
            isInRangeRight = false;
        }

        if (HandIK.Instance.isLeftGrab && isInRangeLeft)
        {
            Debug.Log("left grab");
            // transform.position = HandIK.Instance.leftGripPivot.position;
            transform.SetParent(HandIK.Instance.leftGripPivot);
        }

        else if (HandIK.Instance.isRightGrab && isInRangeRight)
        {
            Debug.Log("right grab");
            // transform.position = HandIK.Instance.rightGripPivot.position;
            transform.SetParent(HandIK.Instance.rightGripPivot);
        }

        else
        {
            transform.SetParent(scene);
        }
    }


}
