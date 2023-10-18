using UnityEngine;
using System;
using System.Collections;
using Microsoft.MixedReality.Toolkit;

[RequireComponent(typeof(Animator))]

public class IKControl : MonoBehaviour {

    protected Animator animator;

    public bool ikActive = true;
    public bool moveHead = true;

    public Transform rightHandObj = null;
    public Transform leftHandObj = null;

    [Range(0f, 1f)]
    [SerializeField] float lookAtWeight = 1f;

    [SerializeField] float lookAtDistance = 100f;

    public float lerpSpeed = 0.1f;
    

    // private Vector3 rightHandPos;
    // private Vector3 leftHandPos;

    // public Vector3 RightHandPos => animator.GetIKPosition(AvatarIKGoal.RightHand);
    // public Vector3 LeftHandPos => animator.GetIKPosition(AvatarIKGoal.LeftHand);

    void Start ()
    {
        animator = GetComponent<Animator>();
    }
    
    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if(animator) {

            //if the IK is active, set the position and rotation directly to the goal.
            if(ikActive) {
                // Set the look target position, if one has been assigned
                if(moveHead)
                {
                    animator.SetLookAtWeight(lookAtWeight);
                    // animator.SetLookAtPosition(new Vector3(0f, Camera.main.transform.forward.y * lookAtDistance, 0f));
                    animator.SetLookAtPosition(Camera.main.transform.forward * lookAtDistance);
                    // animator.SetLookAtPosition(lookAtTransform.position);
                    
                }    

                // Set the right hand target position and rotation, if one has been assigned
                if (rightHandObj != null) {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
                }   
                
                // Set the left hand target position and rotation, if one has been assigned
                if (leftHandObj != null) {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);  
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
                }  
                

            }

            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            // else {          
            //     animator.SetIKPositionWeight(AvatarIKGoal.RightHand,0);
            //     animator.SetIKRotationWeight(AvatarIKGoal.RightHand,0);
            //     animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,0);
            //     animator.SetIKRotationWeight(AvatarIKGoal.LeftHand,0);
            //     animator.SetLookAtWeight(0);
            // }
        }
    }    
}


