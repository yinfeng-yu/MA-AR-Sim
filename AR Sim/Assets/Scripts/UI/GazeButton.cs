using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class GazeButton : MonoBehaviour, IGazeUI
{
    public bool isGazed = false;
    public bool isGazeTriggerable = false;

    public float gazeTriggerTime = 2.5f;
    private float _gazeTime = 0f;

    [Header("Events")]
    public UnityEvent gazeBegin = new UnityEvent();
    public UnityEvent gazeEnd = new UnityEvent();
    public UnityEvent buttonPressed = new UnityEvent();
    public UnityEvent buttonReleased = new UnityEvent();

    public void StartGaze()
    {
        isGazed = true;
        gazeBegin.Invoke();
    }

    private void Update()
    {
        if (isGazeTriggerable)
        {
            if (isGazed)
            {
                _gazeTime += Time.deltaTime;
            }

            if (_gazeTime >= gazeTriggerTime)
            {
                EndGaze();
                buttonPressed.Invoke();
            }
        }
        
    }

    public void EndGaze()
    {
        _gazeTime = 0;
        isGazed = false;
        gazeEnd.Invoke();
    }

    public void Press()
    {
        buttonPressed.Invoke();
    }

    public void Release()
    {
        buttonReleased.Invoke();
    }

}
