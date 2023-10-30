using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitchController : MonoBehaviour
{
    /// <summary>
    /// Controlled lights from the current light switch
    /// </summary>
    [SerializeField] private Light[] _controlledLights;

    /// <summary>
    /// Whether light is currently on
    /// </summary>
    private bool _lightsOn = true;

    public float triggerDistance = 0.3f;

    private bool _isTriggered = false;
    
    private void Update()
    {
        if (Vector3.Distance(FingerController.Instance.leftGripPivot.position, transform.position) <= triggerDistance && !_isTriggered)
        {
            _isTriggered = true;
            SwitchLight();
        }

        if (Vector3.Distance(FingerController.Instance.leftGripPivot.position, transform.position) > triggerDistance && _isTriggered)
        {
            _isTriggered = false;
        }
    }

    private void SwitchLight()
    {
        _lightsOn = !_lightsOn;
        foreach (Light light in _controlledLights)
        {
            light.enabled = _lightsOn;
        }
    }

}
