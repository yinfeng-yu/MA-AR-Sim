using Microsoft.MixedReality.Toolkit.Experimental.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKController : MonoBehaviour
{
    /// <summary>
    /// Root of the armature
    /// </summary>
    public IKJoint root;

    /// <summary>
    /// End effector
    /// </summary>
    public IKJoint end;

    /// <summary>
    /// The IK target that the end joint should follow
    /// </summary>
    public Transform IKTarget;

    /// <summary>
    /// Distance threshold to IKTarget
    /// </summary>
    public float threshold = 0.05f;

    /// <summary>
    /// Rotation speed rate
    /// </summary>
    public float rate = 10f;

    /// <summary>
    /// Number of iteration performed within 1 update
    /// </summary>
    public int steps = 20;

    public bool isROSConstrained = false;

    void Update()
    {
        for (int i = 0; i < steps; ++i)
        {
            if (Vector3.Distance(end.transform.position, IKTarget.position) > threshold)
            {
                IKJoint curJoint = root;
                while (curJoint != null)
                {
                    float slope = CalculateSlope(curJoint);
                    curJoint.Rotate(-slope * rate, isROSConstrained);
                    curJoint = curJoint.GetChild();
                }
            }
        }
        
    }

    float CalculateSlope(IKJoint joint)
    {
        float deltaTheta = 0.01f;
        float dist1 = Vector3.Distance(end.transform.position, IKTarget.position);

        joint.Rotate(deltaTheta, false);
        float dist2 = Vector3.Distance(end.transform.position, IKTarget.position);

        joint.Rotate(-deltaTheta, false);

        return (dist2 - dist1) / deltaTheta;

    }
}
