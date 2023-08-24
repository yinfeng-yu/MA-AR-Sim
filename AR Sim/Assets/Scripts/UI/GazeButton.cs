using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class GazeButton : MonoBehaviour, IGazeUI
{
    [Header("Events")]
    public UnityEvent gazeBegin = new UnityEvent();
    public UnityEvent gazeEnd = new UnityEvent();
    public UnityEvent buttonPressed = new UnityEvent();
    public UnityEvent buttonReleased = new UnityEvent();

    public void Press()
    {
        buttonPressed.Invoke();
    }

    public void Release()
    {
        buttonReleased.Invoke();
    }

}
