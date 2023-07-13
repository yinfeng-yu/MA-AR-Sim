using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public BaseControlMode currentControlMode;

    public FreeLookControlMode freeLookControlMode = new FreeLookControlMode();


    private void Start()
    {
        currentControlMode = freeLookControlMode;
        currentControlMode.EnterControlMode(this);
    }

    private void Update()
    {
        currentControlMode.UpdateControlMode(this);

        // Update the agent state UI.
        // UIManager.instance.agentStateLabel.GetComponentInChildren<LocalizeStringEvent>().StringReference.SetReference("HERA Touch Table", currentState.locRefKey);
    }

    public void SwitchState(BaseControlMode a_controlMode)
    {
        currentControlMode.ExitControlMode(this);
        currentControlMode = a_controlMode;
        a_controlMode.EnterControlMode(this);
    }
}
