using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseState
{

    public IdleState()
    {

    }

    public override void EnterState(StateManager a_stateManager)
    {
        Debug.Log("Start idling.");
    }

    public override void UpdateState(StateManager a_stateManager)
    {

        // if (agentTaskModule.HasTask())
        // {
        // 
        //     if (agentTaskModule.HasCorrectItem())
        //     {
        //         // Skip collecting, head to patient.
        //         agentNavModule.SetDestination(agentTaskModule.GetCurrentTask().targetSiteEnum);
        //         agentStateModule.SwitchState(agentStateModule.displacingState);
        // 
        //     }
        // 
        //     else if (agentTaskModule.HasItem())
        //     {
        //         // Have the wrong item. Need to return first.
        //         agentNavModule.SetDestination(agentTaskModule.GetCurrentItem().returnSiteEnum);
        //         agentStateModule.SwitchState(agentStateModule.displacingState);
        //     }
        // 
        //     else
        //     {
        //         agentNavModule.SetDestination(agentTaskModule.GetCurrentTask().taskData.requiredItem.collectSiteEnum);
        //         // Empty hands. Need to move to collect location and then collect.
        //         agentStateModule.SwitchState(agentStateModule.displacingState);
        //     }
        // }

    }

    public override void ExitState(StateManager a_stateManager)
    {

    }
}
