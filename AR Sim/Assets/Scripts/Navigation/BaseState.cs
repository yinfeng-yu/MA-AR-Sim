using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState
{
    public abstract void EnterState(StateManager a_stateManager);
    public abstract void UpdateState(StateManager a_stateManager);
    public abstract void ExitState(StateManager a_stateManager);
}