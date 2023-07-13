using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;

public class RoboyHands : MonoBehaviour
{

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

    [SerializeField] public Transform roboyLeftHand;
    [SerializeField] public Transform roboyRightHand;
    
    [SerializeField] private String leftHandVisualizer;
    [SerializeField] private String rightHandVisualizer;
    [SerializeField] protected Transform avatarLeftHand;
    [SerializeField] protected Transform avatarRightHand;

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
    public Transform playerLeftHand, playerRightHand;
    
    protected float timeSinceLastSearch;
    protected Vector3 leftHandTargetPosition, rightHandTargetPosition;
    protected Quaternion leftHandTargetRotation, rightHandTargetRotation;
    
    protected IKControl ikcontrol;
    protected GameObject leftGrab;
    protected Quaternion leftRotation;
    protected GameObject rightGrab;
    protected Quaternion rightRotation;


    public Vector3 HandOffset
    {
        get => handOffset;
        set => handOffset = value;
    }
    public Transform PlayerLeftHand => playerLeftHand;
    public Transform PlayerRightHand => playerRightHand;
    public GameObject LeftGrab
    {
        get => leftGrab;
        set
        {
            leftGrab = value;
            if(value != null) leftRotation = leftGrab.transform.rotation;
        }
    }

    public GameObject RightGrab
    {
        get => rightGrab;
        set
        {
            rightGrab = value;
            if(value != null) rightRotation = rightGrab.transform.rotation;

        }
    }

    protected virtual void Start()
    {
        ikcontrol = GetComponent<IKControl>();
        initialAvatarLeftHandRotation = Quaternion.Inverse(transform.rotation) * avatarLeftHand.rotation;
        initialAvatarRightHandRotation = Quaternion.Inverse(transform.rotation) * avatarRightHand.rotation;

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

        HandsFollowing();

    }

    /// <summary>
    /// Roboy's IK targets follow the supported input sources.
    /// </summary>
    void HandsFollowing()
    {
        // Controller
        foreach (IMixedRealityController controller in CoreServices.InputSystem.DetectedControllers)
        {
            // Debug.Log(controller.InputSource.SourceType);

            // If detected controller is supported
            if (supportedInputSourceTypes.Contains(controller.InputSource.SourceType))
            {
                // Interactions for a controller is the list of inputs that this controller exposes
                foreach (MixedRealityInteractionMapping interactionMapping in controller.Interactions)
                {
                    // 6DOF controllers support the "SpatialPointer" type (pointing direction)
                    // or "GripPointer" type (direction of the 6DOF controller)
                    if (interactionMapping.InputType == DeviceInputType.SpatialPointer)
                    {
                        // Debug.Log("Spatial pointer PositionData: " + interactionMapping.PositionData);
                        // Debug.Log("Spatial pointer RotationData: " + interactionMapping.RotationData);
                        avatarLeftHand.position = interactionMapping.PositionData;
                        avatarLeftHand.rotation = interactionMapping.RotationData * Quaternion.Euler(Vector3.forward * leftHandRotationZOffset);
                    }

                    if (interactionMapping.InputType == DeviceInputType.SpatialGrip)
                    {
                        // Debug.Log("Spatial grip PositionData: " + interactionMapping.PositionData);
                        // Debug.Log("Spatial grip RotationData: " + interactionMapping.RotationData);
                    }
                }
            }
        }
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
                    leftGrab.GetComponentInChildren<RoboyGrabbable>().LetGo(handsClosed);
                    leftGrab = null;
                }

                if (rightGrab != null)
                {
                    rightGrab.GetComponentInChildren<RoboyGrabbable>().LetGo(handsClosed);
                    rightGrab = null;
                }
                
            }
           
        }
        
    }
}