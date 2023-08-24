using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum ControlMode
{
    MainMenu,
    MLController,
    Smartphone,
    HandTracking,
}

public class ControlModeManager : MonoBehaviour
{
    #region Singleton
    public static ControlModeManager instance;
    private void Awake()
    {
        if (instance != this)
        {
            instance = this;
        }
    }
    #endregion

    public SmartPhoneController smartPhoneController;
    public CameraManager cameraManager;

    BaseControlMode currentControlMode;

    public ControlMode currentControlModeEnum;

    MainMenuControlMode mainMenuControlMode = new MainMenuControlMode();
    SmartphoneControlMode smartPhoneControlMode = new SmartphoneControlMode();
    HandTrackingControlMode handTrackingControlMode = new HandTrackingControlMode();


    private void Start()
    {
        currentControlMode = mainMenuControlMode;
        currentControlModeEnum = ControlMode.MainMenu;
        cameraManager.ActivateCamera(currentControlModeEnum);

        currentControlMode.EnterControlMode(this);
    }

    private void Update()
    {
        currentControlMode.UpdateControlMode(this);

    }

    // public void SwitchState(BaseControlMode a_controlMode)
    // {
    //     currentControlMode.ExitControlMode(this);
    //     currentControlMode = a_controlMode;
    // 
    //     cameraManager.ActivateCamera(currentControlModeEnum);
    // 
    //     a_controlMode.EnterControlMode(this);
    // }

    public void SwitchState(ControlMode a_controlModeEnum)
    {
        currentControlMode.ExitControlMode(this);
        
        switch (a_controlModeEnum)
        {
            case ControlMode.MainMenu:
                currentControlMode = mainMenuControlMode;
                break;
            case ControlMode.Smartphone:
                currentControlMode = smartPhoneControlMode;
                break;
            case ControlMode.HandTracking:
                currentControlMode = handTrackingControlMode;
                break;
            default:
                break;
        }
        currentControlModeEnum = a_controlModeEnum;

        cameraManager.ActivateCamera(currentControlModeEnum);

        currentControlMode.EnterControlMode(this);
    }

    public void SwitchToSmartphone() => SwitchState(ControlMode.Smartphone);
    public void SwitchToHandTracking() => SwitchState(ControlMode.HandTracking);
    public void SwitchToMainMenu() => SwitchState(ControlMode.MainMenu);
}
