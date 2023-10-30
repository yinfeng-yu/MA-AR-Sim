using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class ToastManager : Singleton<ToastManager>
{
    [SerializeField] private GameObject _toast;
    [SerializeField] private TextMeshPro _toastText;

    public float lifetime;
    private float _elapsedTime = 0f;
    private bool _isToastShown = false;
    
    public void ShowToast(string text, float timeout)
    {
        if (_toast != null)
        {
            _toast.SetActive(true);
        }

        _isToastShown = true;

        lifetime = timeout;
        _toastText.text = text;
    }

    void HideToast()
    {
        if (_toast != null)
        {
            _toast.SetActive(false);
        }
    }


    void Start()
    {
        _toast = GameObject.Find("Toast");
        _toastText = _toast.GetComponentInChildren<TextMeshPro>();
        _toast.SetActive(false);
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
