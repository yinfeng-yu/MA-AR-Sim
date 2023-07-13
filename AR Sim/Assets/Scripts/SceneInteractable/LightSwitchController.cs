using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitchController : MonoBehaviour
{
    /// <summary>
    /// Controlled light from the current light switch
    /// </summary>
    [SerializeField] private Light _controlledLight;

    /// <summary>
    /// Whether light is currently on
    /// </summary>
    private bool _lightOn => _controlledLight.gameObject.activeSelf;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Left") || other.gameObject.CompareTag("Right"))
        {
            _controlledLight.gameObject.SetActive(!_lightOn);
        }
    }

}
