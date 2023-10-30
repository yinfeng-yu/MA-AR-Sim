using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum ControlMode
{
    MainMenu,
    Smartphone,
    SmartphonePointer,
    SmartphonePoseCalibration,
    HandTracking,
    Steer,
    Test,
}

public class ControlModeManager : Singleton<ControlModeManager>
{

    // public SmartPhoneController smartPhoneController;
    public CameraManager cameraManager;

    BaseControlMode _currentControlMode;
    public ControlMode currentControlMode = ControlMode.MainMenu;

    MainMenuControlMode mainMenuControlMode = new MainMenuControlMode();
    SmartphoneControlMode smartphoneControlMode = new SmartphoneControlMode();
    SmartphonePoseCalibrationControlMode smartphonePoseCalibrationMode = new SmartphonePoseCalibrationControlMode();
    SmartphonePointerControlMode smartphonePointerControlMode = new SmartphonePointerControlMode();
    HandTrackingControlMode handTrackingControlMode = new HandTrackingControlMode();
    SteerControlMode steerControlMode = new SteerControlMode();
    TestControlMode testControlMode = new TestControlMode();

    private void Start()
    {
        _currentControlMode = mainMenuControlMode;
        // cameraManager.ActivateCamera(CameraType.MainMenu);

        _currentControlMode.EnterControlMode(this);
    }

    private void Update()
    {
        _currentControlMode.UpdateControlMode(this);

    }

    public void SwitchState(ControlMode controlMode)
    {
        // Debug.Log($"switch to {controlMode.ToString()}");
        _currentControlMode.ExitControlMode(this);
        
        switch (controlMode)
        {
            case ControlMode.MainMenu:
                _currentControlMode = mainMenuControlMode;
                cameraManager.ActivateCamera(CameraType.MainMenu);
                break;
            case ControlMode.Smartphone:
                _currentControlMode = smartphoneControlMode;
                cameraManager.ActivateCamera(CameraType.FirstPerson);
                break;
            case ControlMode.SmartphonePoseCalibration:
                _currentControlMode = smartphonePoseCalibrationMode;
                cameraManager.ActivateCamera(CameraType.FirstPerson);
                break;
            case ControlMode.SmartphonePointer:
                _currentControlMode = smartphonePointerControlMode;
                cameraManager.ActivateCamera(CameraType.FirstPerson);
                break;
            case ControlMode.HandTracking:
                _currentControlMode = handTrackingControlMode;
                cameraManager.ActivateCamera(CameraType.FirstPerson);
                break;
            case ControlMode.Steer:
                _currentControlMode = steerControlMode;
                cameraManager.ActivateCamera(CameraType.FirstPerson);
                break;
            case ControlMode.Test:
                _currentControlMode = testControlMode;
                cameraManager.ActivateCamera(CameraType.FirstPerson);
                break;
            default:
                break;
        }
        OperationSender.Instance.SendSwitchControlMode(controlMode);

        currentControlMode = controlMode;
        _currentControlMode.EnterControlMode(this);
    }

    public void SwitchToSmartphone() => SwitchState(ControlMode.Smartphone);
    public void SwitchToSmartphonePoseCalibration() => SwitchState(ControlMode.SmartphonePoseCalibration);
    public void SwitchToSmartphonePointer() => SwitchState(ControlMode.SmartphonePointer);
    public void SwitchToHandTracking() => SwitchState(ControlMode.HandTracking);
    public void SwitchToSteer() => SwitchState(ControlMode.Steer);
    public void SwitchToTest() => SwitchState(ControlMode.Test);
    public void SwitchToMainMenu() => SwitchState(ControlMode.MainMenu);
}
