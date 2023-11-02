using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class ToastManager : Singleton<ToastManager>
{
    [SerializeField] private Toast _toast;

    public float lifetime;
    private float _elapsedTime = 0f;
    private bool _isToastShown = false;
    
    public void ShowToast(string text, float timeout)
    {
        if (_toast != null)
        {
            _toast.ShowToast();
            _toast.SetToastText(text);
        }

        _isToastShown = true;

        lifetime = timeout;
    }

    void HideToast()
    {
        if (_toast != null)
        {
            _toast.HideToast();
        }

        _isToastShown = false;
    }


    void Start()
    {
        _toast = FindObjectOfType<Toast>();
        HideToast();
    }

    void Update()
    {
        if (_isToastShown)
        {
            _elapsedTime += Time.deltaTime;
        }

        if (_elapsedTime >= lifetime) 
        {
            _elapsedTime = 0f;
            HideToast();
        }
    }


}
