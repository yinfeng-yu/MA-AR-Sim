using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedDownAdjustmentController : MonoBehaviour
{
    
    /// <summary>
    /// Reference to bed controller
    /// </summary>
    private BedAdjustmentController _controller;

    private void OnEnable()
    {
        _controller = GetComponentInParent<BedAdjustmentController>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Left") || other.gameObject.CompareTag("Right"))
        {
            _controller.MoveBedDown();
        }
    }

}
