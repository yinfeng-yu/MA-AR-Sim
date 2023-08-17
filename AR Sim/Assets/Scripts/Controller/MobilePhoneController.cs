using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobilePhoneController : MonoBehaviour
{
    [SerializeField] Transform _mobilePhonePointer;
    [SerializeField] Transform _pointerLine;
    [SerializeField] float leftHandRotationZOffset = 90;

    Quaternion _deviceOrientation;
    float _deviceRange;

    [SerializeField] Transform _leftHandIKTarget;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TransmissionManager.instance.globalQuaternions.TryGetValue("deviceOrientation", out _deviceOrientation);
        // _deviceOrientation = TransmissionManager.instance.globalQuaternions["deviceOrientation"];
        TransmissionManager.instance.globalFloats.TryGetValue("deviceRange", out _deviceRange);
        // _deviceRange = TransmissionManager.instance.globalFloats["deviceRange"];

        _mobilePhonePointer.rotation = _deviceOrientation;
        _pointerLine.rotation = _deviceOrientation;
        // _rangeSphere.localScale = new Vector3(_deviceRange, _deviceRange, _deviceRange);
        _pointerLine.localScale = new Vector3(_pointerLine.localScale.x, _pointerLine.localScale.y, _deviceRange);

        _leftHandIKTarget.rotation = _mobilePhonePointer.rotation * Quaternion.Euler(Vector3.forward * leftHandRotationZOffset);
        _leftHandIKTarget.position = transform.position + _mobilePhonePointer.forward * _deviceRange;

        // Debug.Log($"device orientation: {_deviceOrientation}, device range: {_deviceRange}");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_mobilePhonePointer.position, _mobilePhonePointer.position + _mobilePhonePointer.forward * 5f);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_mobilePhonePointer.position, _mobilePhonePointer.position + _mobilePhonePointer.right * 5f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(_mobilePhonePointer.position, _mobilePhonePointer.position + _mobilePhonePointer.up * 5f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_mobilePhonePointer.position, _deviceRange);
    }
}
