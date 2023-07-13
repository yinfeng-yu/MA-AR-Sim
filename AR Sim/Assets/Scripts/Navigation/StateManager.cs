using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public BaseState currentState;

    // public AgentDisplacingState displacingState = new AgentDisplacingState();
    // public AgentInteractingState interactingState = new AgentInteractingState();
    public IdleState idleState = new IdleState();


    private void Start()
    {
        currentState = idleState;
        currentState.EnterState(this);
    }

    private void Update()
    {
        currentState.UpdateState(this);

        // Update the agent state UI.
        // UIManager.instance.agentStateLabel.GetComponentInChildren<LocalizeStringEvent>().StringReference.SetReference("HERA Touch Table", currentState.locRefKey);
    }

    public void SwitchState(BaseState state)
    {
        currentState.ExitState(this);
        currentState = state;
        state.EnterState(this);
    }
}