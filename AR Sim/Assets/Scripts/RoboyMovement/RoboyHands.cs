using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

public class RoboyHands : MonoBehaviour
{
    public bool isLeftHandGrabbing = false;
    public bool isRightHandGrabbing = false;

    public Handedness handedness = Handedness.Left;
    
    public bool handsClosed = false;
    public event EventHandler<bool> OnHandsClosing;
    public bool HandsClosed
    {
        get => handsClosed;
        set
        {
            handsClosed = value;
            OnHandsClosing?.Invoke(this, handsClosed);
        }
    }

    public Vector3 handOffset;

    /// <summary>
    /// Specifies how quickly the avatar moves from one position to the next
    /// This value is required in order to get smooth transitions
    /// </summary>
    public float lerpSpeed = 0.1f;
        
    [SerializeField] protected Transform leftHandIKTarget;
    [SerializeField] protected Transform rightHandIKTarget;

    [SerializeField] List<InputSourceType> supportedInputSourceTypes;
    [SerializeField] float leftHandRotationZOffset = 90;

    /// <summary>
    /// whether roboy's hand should updated to user's hand's position
    /// </summary>
    [SerializeField] public bool handUpdate = false;

    /// <summary>
    /// bool for whether roboy hands moved
    /// </summary>
    [SerializeField] public bool handsMoved = false;

    protected Quaternion initialAvatarLeftHandRotation;
    protected Quaternion initialAvatarRightHandRotation;
    
    protected float timeSinceLastSearch;
    protected Vector3 leftHandTargetPosition, rightHandTargetPosition;
    protected Quaternion leftHandTargetRotation, rightHandTargetRotation;
    
    protected IKControl ikcontrol;
    protected GameObject leftGrab;
    protected Quaternion leftRotation;
    protected GameObject rightGrab;
    protected Quaternion rightRotation;

    private bool _leftHandUpdate = false;
    public bool leftHandUpdate
    {
        get => _leftHandUpdate;
        set => _leftHandUpdate = value;        
    }

    public Vector3 HandOffset
    {
        get => handOffset;
        set => handOffset = value;
    }

    // public GameObject LeftGrab
    // {
    //     get => leftGrab;
    //     set
    //     {
    //         leftGrab = value;
    //         if(value != null) leftRotation = leftGrab.transform.rotation;
    //     }
    // }
    // 
    // public GameObject RightGrab
    // {
    //     get => rightGrab;
    //     set
    //     {
    //         rightGrab = value;
    //         if(value != null) rightRotation = rightGrab.transform.rotation;
    // 
    //     }
    // }

    protected virtual void Start()
    {
        ikcontrol = GetComponent<IKControl>();
        initialAvatarLeftHandRotation = Quaternion.Inverse(transform.rotation) * leftHandIKTarget.rotation;
        initialAvatarRightHandRotation = Quaternion.Inverse(transform.rotation) * rightHandIKTarget.rotation;

        if (supportedInputSourceTypes.Count == 0)
        {
            Debug.LogWarning("No input source type assigned. By default controller is supported.");
            supportedInputSourceTypes = new List<InputSourceType>();
            supportedInputSourceTypes.Add(InputSourceType.Controller);
        }
    }


    protected virtual void Update()
    {

        // MoveGrabbedObject(leftGrab, avatarLeftHand, roboyLeftHand.transform.position, leftRotation);
        // MoveGrabbedObject(rightGrab, avatarRightHand, roboyRightHand.transform.position, rightRotation);
        // 
        // if (handUpdate)
        //     SearchHands();
        // else
        // {
        //     playerLeftHand = null;
        //     playerRightHand = null;
        // }
        // 
        // leftHandTargetPosition = EncodeHandPosition(playerLeftHand);
        // leftHandTargetRotation = EncodeHandRotation(playerLeftHand);
        // rightHandTargetPosition = EncodeHandPosition(playerRightHand);
        // rightHandTargetRotation = EncodeHandRotation(playerRightHand);
        // 
        // MoveAvatarHand(avatarLeftHand, leftHandTargetPosition, leftHandTargetRotation, initialAvatarLeftHandRotation);
        // MoveAvatarHand(avatarRightHand, rightHandTargetPosition, rightHandTargetRotation,
        //     initialAvatarRightHandRotation);

    }

    public void LeftGrab()
    {
        GetComponentInChildren<Animator>().SetBool("grab_left", true);
        isLeftHandGrabbing = true;
    }

    public void LeftRelease()
    {
        GetComponentInChildren<Animator>().SetBool("grab_left", false);
        isLeftHandGrabbing = false;
    }

    public void RightGrab()
    {
        GetComponentInChildren<Animator>().SetBool("grab_right", true);
        isRightHandGrabbing = true;
    }

    public void RightRelease()
    {
        GetComponentInChildren<Animator>().SetBool("grab_right", false);
        isRightHandGrabbing = false;
    }


    /// <summary>
    /// Encodes the hand position for networking
    /// In order to keep the network messages small, a non-tracked hand is represented by the 0-vector (it is very unlikely that the user will position the hand exactly in this position)
    /// </summary>
    /// <param name="hand">The hand to encode</param>
    /// <returns>The position of the hand if it is tracked or the 0-vector otherwise</returns>
    protected Vector3 EncodeHandPosition(Transform hand)
    {
        if (hand == null)
        {
            return Vector3.zero;
        }
        else
        {
            return hand.position + handOffset;
        }
    }

    protected Quaternion EncodeHandRotation(Transform hand)
    {
        if (hand == null)
        {
            return Quaternion.identity;
        }
        else
        {
            return hand.rotation;
        }
    }


    protected void MoveAvatarHand(Transform handAvatar, Vector3 handTargetPosition, Quaternion handTargetRotation,
        Quaternion initialHandAvatarRotation)
    {
        if (handTargetPosition == Vector3.zero && handTargetRotation == Quaternion.identity)
        {
            handAvatar.gameObject.SetActive(false);
        }
        else
        {
            handAvatar.gameObject.SetActive(true);

            handAvatar.transform.position =
                Vector3.Lerp(handAvatar.position, handTargetPosition, lerpSpeed * Time.deltaTime);
            handTargetRotation = handTargetRotation * initialHandAvatarRotation;
            handAvatar.transform.rotation =
                Quaternion.Slerp(handAvatar.rotation, handTargetRotation, lerpSpeed * Time.deltaTime);
            handsMoved = true;
        }
    }

    public void MoveGrabbedObject(GameObject grabbedObject, Transform handAvatar, Vector3 handPos, Quaternion initialRotation)
    {
        if (grabbedObject != null)
        {
            if (handsClosed)
            {
                // grabbedObject.transform.position =
                //     Vector3.Lerp(grabbedObject.transform.position, handPos, lerpSpeed * Time.deltaTime);
                //grabbedObject.transform.rotation = handAvatar.rotation * initialRotation;
                //grabbedObject.transform.rotation =
                //Quaternion.Slerp(grabbedObject.transform.rotation, handAvatar.rotation, lerpSpeed * Time.deltaTime);
                grabbedObject.transform.position = handPos;
            }
            else
            {
                if (leftGrab != null)
                {
                    // leftGrab.GetComponentInChildren<RoboyGrabbable>().LetGo(handsClosed);
                    leftGrab = null;
                }

                if (rightGrab != null)
                {
                    // rightGrab.GetComponentInChildren<RoboyGrabbable>().LetGo(handsClosed);
                    rightGrab = null;
                }
                
            }
           
        }
        
    }

}