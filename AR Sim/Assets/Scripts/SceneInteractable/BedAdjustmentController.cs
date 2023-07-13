using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(BedAdjustmentController)), CanEditMultipleObjects]
internal class BedAdjustmentControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        BedAdjustmentController _controller = (BedAdjustmentController)target;

        // Add OnTriggerEnter to up and down buttons
        GameObject up = _controller.transform.Find("Up").gameObject;
        GameObject down = _controller.transform.Find("Down").gameObject;

        if (!up) Debug.LogError("Up button could not be found!");
        if (!down) Debug.LogError("Down button could not be found!");

        if (!up.GetComponent<BedUpAdjustmentController>())
            up.AddComponent<BedUpAdjustmentController>();
        if (!down.GetComponent<BedDownAdjustmentController>())
            down.AddComponent<BedDownAdjustmentController>();

        if (!up.GetComponent<BoxCollider>()) up.AddComponent<BoxCollider>();
        if (!down.GetComponent<BoxCollider>()) down.AddComponent<BoxCollider>();
    }
}
#endif

public class BedAdjustmentController : MonoBehaviour
{
    /// <summary>
    /// Instance of the bed
    /// </summary>
    [SerializeField] private GameObject _bedInstance;

    /// <summary>
    /// Instance of the patient
    /// </summary>
    [SerializeField] private GameObject _patient;

    /// <summary>
    /// Bed adjustment speed
    /// </summary>
    [SerializeField] private float _movementSpeed;

    /// <summary>
    /// Movement in total
    /// </summary>
    private float _wholeMovement = 0f;

    /// <summary>
    /// Move bed up
    /// </summary>
    public void MoveBedUP()
    {
        if (_wholeMovement < 0.1)
        {
            _bedInstance.transform.position += new Vector3(0, _movementSpeed, 0);
            _patient.transform.position += new Vector3(0, _movementSpeed, 0);
            _wholeMovement += _movementSpeed;
        }
    }

    /// <summary>
    /// Move bed down
    /// </summary>
    public void MoveBedDown()
    {
        if (_wholeMovement > -0.1)
        {
            _bedInstance.transform.position -= new Vector3(0, _movementSpeed, 0);
            _patient.transform.position -= new Vector3(0, _movementSpeed, 0);
            _wholeMovement -= _movementSpeed;
        }
    }
}
