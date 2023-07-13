using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;

public class LookAtSolver : Solver
{ 
    public override void SolverUpdate()
    {
        
        Vector3 dirToTarget = (SolverHandler.TransformTarget.position - transform.position).normalized;
        Quaternion desiredRot = Quaternion.LookRotation(transform.position - dirToTarget);
       
        GoalRotation = desiredRot;
    }
}
    
