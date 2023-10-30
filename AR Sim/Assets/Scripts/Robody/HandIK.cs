using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class OldHandBones
{
    public Quaternion[] ThumbDefault = new[]  { Quaternion.Euler(0, 90, 30),     Quaternion.Euler(0, 0, 0),      Quaternion.Euler(0, 0, 0) };
    public Quaternion[] IndexDefault = new[]  { Quaternion.Euler(0, -90, 0),       Quaternion.Euler(0, 0, 0),      Quaternion.Euler(0, 0, 0) };
    public Quaternion[] MiddleDefault = new[] { Quaternion.Euler(0, -90, 0),       Quaternion.Euler(0, 0, 0),      Quaternion.Euler(0, 0, 0) };
    public Quaternion[] RingDefault = new[]   { Quaternion.Euler(0, -90, 0),       Quaternion.Euler(0, 0, 0),      Quaternion.Euler(0, 0, 0) };
    public Quaternion[] PinkyDefault = new[]  { Quaternion.Euler(0, -90, 0),       Quaternion.Euler(0, 0, 0),      Quaternion.Euler(0, 0, 0) };

    public Quaternion[] ThumbGrip = new[]  { Quaternion.Euler(60, 270, 0),      Quaternion.Euler(0, 0, 40),     Quaternion.Euler(0, 0, 80) };
    public Quaternion[] IndexGrip = new[]  { Quaternion.Euler(280, 270, 80),         Quaternion.Euler(270, 0, 0),     Quaternion.Euler(300, 0, 0) };
    public Quaternion[] MiddleGrip = new[] { Quaternion.Euler(280, 270, 80),         Quaternion.Euler(270, 0, 0),     Quaternion.Euler(300, 0, 0) };
    public Quaternion[] RingGrip = new[]   { Quaternion.Euler(280, 270, 80),         Quaternion.Euler(270, 0, 0),     Quaternion.Euler(300, 0, 0) };
    public Quaternion[] PinkyGrip = new[]  { Quaternion.Euler(280, 270, 80),         Quaternion.Euler(270, 0, 0),     Quaternion.Euler(300, 0, 0) };

    public Quaternion[] ThumbAngles = new[] { Quaternion.Euler(0, 90, 30), Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 0) };
    public Quaternion[] IndexAngles = new[] { Quaternion.Euler(0, 90, 0), Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 0) };
    public Quaternion[] MiddleAngles = new[] { Quaternion.Euler(0, 90, 0), Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 0) };
    public Quaternion[] RingAngles = new[] { Quaternion.Euler(0, 90, 0), Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 0) };
    public Quaternion[] PinkyAngles = new[] { Quaternion.Euler(0, 90, 0), Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 0) };

    public static Dictionary<string, HumanBodyBones[]> fingerBones = new Dictionary<string, HumanBodyBones[]>()
    {
        { "rightThumb", new []{ HumanBodyBones.RightThumbProximal, HumanBodyBones.RightThumbIntermediate, HumanBodyBones.RightThumbDistal }},
        { "rightIndex", new []{ HumanBodyBones.RightIndexProximal, HumanBodyBones.RightIndexIntermediate, HumanBodyBones.RightIndexDistal }},
        { "rightMiddle", new []{ HumanBodyBones.RightMiddleProximal, HumanBodyBones.RightMiddleIntermediate, HumanBodyBones.RightMiddleDistal }},
        { "rightRing", new []{ HumanBodyBones.RightRingProximal, HumanBodyBones.RightRingIntermediate, HumanBodyBones.RightRingDistal }},
        { "rightPinky", new []{ HumanBodyBones.RightLittleProximal, HumanBodyBones.RightLittleIntermediate, HumanBodyBones.RightLittleDistal }},

        { "leftThumb", new []{ HumanBodyBones.LeftThumbProximal, HumanBodyBones.LeftThumbIntermediate, HumanBodyBones.LeftThumbDistal }},
        { "leftIndex", new []{ HumanBodyBones.LeftIndexProximal, HumanBodyBones.LeftIndexIntermediate, HumanBodyBones.LeftIndexDistal }},
        { "leftMiddle", new []{ HumanBodyBones.LeftMiddleProximal, HumanBodyBones.LeftMiddleIntermediate, HumanBodyBones.LeftMiddleDistal }},
        { "leftRing", new []{ HumanBodyBones.LeftRingProximal, HumanBodyBones.LeftRingIntermediate, HumanBodyBones.LeftRingDistal }},
        { "leftPinky", new []{ HumanBodyBones.LeftLittleProximal, HumanBodyBones.LeftLittleIntermediate, HumanBodyBones.LeftLittleDistal }},
    };

    public Animator animator;
    public string hand = "";

    public void UnGrip(string finger)
    {
        int i = 0;
        foreach (var bone in fingerBones[finger])
        {
            try
            {
                if (finger.Contains("Thumb"))
                {
                    animator.SetBoneLocalRotation(bone, ThumbDefault[i]);
                }
                else if (finger.Contains("Index"))
                {
                    animator.SetBoneLocalRotation(bone, IndexDefault[i]);
                }
                else if (finger.Contains("Middle"))
                {
                    animator.SetBoneLocalRotation(bone, MiddleDefault[i]);
                }
                else if (finger.Contains("Ring"))
                {
                    animator.SetBoneLocalRotation(bone, RingDefault[i]);
                }
                else if (finger.Contains("Pinky"))
                {
                    animator.SetBoneLocalRotation(bone, PinkyDefault[i]);
                }
            }
            catch { }
            i++;
        }
    }

    public void Grip(float percentage)
    {
        Grip(hand + "Thumb",  percentage);
        Grip(hand + "Index",  percentage);
        Grip(hand + "Middle", percentage);
        Grip(hand + "Ring",   percentage);
        Grip(hand + "Pinky",  percentage);
    }

    public void Grip(string finger, float percentage)
    {
        int i = 0;
        float sign = finger.Contains("left") ? 1 : -1;
        
        foreach (var bone in fingerBones[finger])
        {
            try
            {
                // Debug.Log($"th: {ThumbGrip.Length}, in: {IndexGrip.Length}, mi: {MiddleGrip.Length}, ri: {RingGrip.Length}, pi:{PinkyGrip.Length}");
                if (finger.Contains("Thumb"))
                {
                    // var tmp = animator.GetBoneTransform(bone).rotation;
                    // Debug.Log($"finger: {finger}, percentage: {percentage}, before: {tmp}, animator == null: {animator == null}");
                   
                    animator.SetBoneLocalRotation(bone, Quaternion.Lerp(ThumbDefault[i], ThumbGrip[i], percentage));

                    // Debug.Log($"finger: {finger}, percentage: {percentage}, before: {tmp}, after: {animator.GetBoneTransform(bone).rotation}");
                }
                else if (finger.Contains("Index"))
                {

                    animator.SetBoneLocalRotation(bone, Quaternion.Lerp(IndexDefault[i], IndexGrip[i], percentage));
                }
                else if (finger.Contains("Middle"))
                {
                    animator.SetBoneLocalRotation(bone, Quaternion.Lerp(MiddleDefault[i], MiddleGrip[i], percentage));
                }
                else if (finger.Contains("Ring"))
                {
                    animator.SetBoneLocalRotation(bone, Quaternion.Lerp(RingDefault[i], RingGrip[i], percentage));
                }
                else if (finger.Contains("Pinky"))
                {
                    animator.SetBoneLocalRotation(bone, Quaternion.Lerp(PinkyDefault[i], PinkyGrip[i], percentage));
                }
            }
            catch(Exception e) { Debug.Log(e); }
            i++;
        }
    }
}

public class HandIK : MonoBehaviour
{
    private Animator animator;
    private float leftHandPosWeight = 1;
    private float rightHandPosWeight = 1;

    // [Range(0, 1)]
    // public float gripPercentage;

    [Header("Left Fingers Percentage")]
    [Range(0, 1)]
    public float leftThumbGripPercentage;
    [Range(0, 1)]
    public float leftIndexGripPercentage;
    [Range(0, 1)]
    public float leftMiddleGripPercentage;
    [Range(0, 1)]
    public float leftRingGripPercentage;
    [Range(0, 1)]
    public float leftPinkyGripPercentage;

    [Header("Right Fingers Percentage")]
    [Range(0, 1)]
    public float rightThumbGripPercentage;
    [Range(0, 1)]
    public float rightIndexGripPercentage;
    [Range(0, 1)]
    public float rightMiddleGripPercentage;
    [Range(0, 1)]
    public float rightRingGripPercentage;
    [Range(0, 1)]
    public float rightPinkyGripPercentage;

    private Transform RightHandBone;
    private Transform LeftHandBone;

    public OldHandBones LeftHandBones;
    public OldHandBones RightHandBones;

    public bool fingerTracking = true;
    public bool fingerPercentage = true;

    public bool isLeftGrab = false;
    public bool isRightGrab = false;

    public float leftPercentage = 0f;
    public float rightPercentage = 0f;

    public float grabTime = 0.5f;
    float _leftGrabTime = 0;
    float _rightGrabTime = 0;

    [Range(0, 1)]
    public float grabThreshold = 0.7f;

    public Transform leftGripPivot;
    public Transform rightGripPivot;


    //public BoneMap LeftHandIK;
    //public BoneMap RightHandIK;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        RightHandBone = animator.GetBoneTransform(HumanBodyBones.RightHand);
        LeftHandBone = animator.GetBoneTransform(HumanBodyBones.LeftHand);

        LeftHandBones.ThumbAngles[0] = Quaternion.Euler(0, -90, -30);
        LeftHandBones.IndexAngles[0] = Quaternion.Euler(0, -90, 0);
        LeftHandBones.MiddleAngles[0] = Quaternion.Euler(0, -90, 0);
        LeftHandBones.RingAngles[0] = Quaternion.Euler(0, -90, 0);
        LeftHandBones.PinkyAngles[0] = Quaternion.Euler(0, -90, 0);

        RightHandBones.ThumbAngles[0] = Quaternion.Euler(0, 90, 30);
        RightHandBones.IndexAngles[0] = Quaternion.Euler(0, 90, 0);
        RightHandBones.MiddleAngles[0] = Quaternion.Euler(0, 90, 0);
        RightHandBones.RingAngles[0] = Quaternion.Euler(0, 90, 0);
        RightHandBones.PinkyAngles[0] = Quaternion.Euler(0, 90, 0);


        RightHandBones.animator = animator;
        RightHandBones.hand = "right";

        LeftHandBones.animator = animator;
        LeftHandBones.hand = "left";

    }

    private void OnAnimatorIK(int layerIndex)
    {
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandPosWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandPosWeight);

        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandPosWeight);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandPosWeight);

        // LeftHandIK.Map();
        // RightHandIK.Map();

        // animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandIK.rigTarget.transform.position);
        // animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandIK.rigTarget.transform.position);

        /*
        animator.SetBoneLocalRotation(HumanBodyBones.RightMiddleIntermediate, Quaternion.Euler(0, 0, 0));
        animator.SetBoneLocalRotation(HumanBodyBones.RightMiddleDistal, Quaternion.Euler(0, 0, 0));
        */

        if (fingerTracking)
        {
            if (fingerPercentage)
            {
                UpdateFingersPercentage();

                if (leftThumbGripPercentage > grabThreshold && 
                    leftIndexGripPercentage > grabThreshold && 
                    leftMiddleGripPercentage > grabThreshold && 
                    leftRingGripPercentage > grabThreshold && 
                    leftPinkyGripPercentage > grabThreshold)
                {
                    isLeftGrab = true;
                }
                else
                {
                    isLeftGrab = false;
                }

                if (rightThumbGripPercentage > grabThreshold &&
                    rightIndexGripPercentage > grabThreshold &&
                    rightMiddleGripPercentage > grabThreshold &&
                    rightRingGripPercentage > grabThreshold &&
                    rightPinkyGripPercentage > grabThreshold)
                {
                    isRightGrab = true;
                }
                else
                {   
                    isRightGrab = false;
                }
            }
            else
            {
                UpdateFingers();
            }
        }
        else
        {
            if (isLeftGrab)
            {
                if (_leftGrabTime < grabTime)
                {
                    _leftGrabTime += Time.deltaTime;
                }
                else
                {
                    _leftGrabTime = grabTime;
                }

                leftPercentage = Mathf.Lerp(0, 1, _leftGrabTime / grabTime);

                LeftHandBones.Grip(leftPercentage);
            }
            else
            {
                if (_leftGrabTime > 0)
                {
                    _leftGrabTime -= Time.deltaTime;
                }
                else
                {
                    _leftGrabTime = 0;
                }

                leftPercentage = Mathf.Lerp(0, 1, _leftGrabTime / grabTime);

                LeftHandBones.Grip(leftPercentage);
            }

            if (isRightGrab)
            {
                if (_rightGrabTime < grabTime)
                {
                    _rightGrabTime += Time.deltaTime;
                }
                else
                {
                    _rightGrabTime = grabTime;
                }

                rightPercentage = Mathf.Lerp(0, 1, _rightGrabTime / grabTime);

                RightHandBones.Grip(rightPercentage);
            }
            else
            {
                if (_rightGrabTime > 0)
                {
                    _rightGrabTime -= Time.deltaTime;
                }
                else
                {
                    _rightGrabTime = 0;
                }

                rightPercentage = Mathf.Lerp(0, 1, _rightGrabTime / grabTime);

                RightHandBones.Grip(rightPercentage);
            }
        }

    }

    void UpdateFingersPercentage()
    {
        UpdateFingerPercentage("rightThumb");
        UpdateFingerPercentage("rightIndex");
        UpdateFingerPercentage("rightMiddle");
        UpdateFingerPercentage("rightRing");
        UpdateFingerPercentage("rightPinky");

        UpdateFingerPercentage("leftThumb");
        UpdateFingerPercentage("leftIndex");
        UpdateFingerPercentage("leftMiddle");
        UpdateFingerPercentage("leftRing");
        UpdateFingerPercentage("leftPinky");
    }

    void UpdateFingers()
    {
        UpdateFinger("rightThumb");
        UpdateFinger("rightIndex");
        UpdateFinger("rightMiddle");
        UpdateFinger("rightRing");
        UpdateFinger("rightPinky");

        UpdateFinger("leftThumb");
        UpdateFinger("leftIndex");
        UpdateFinger("leftMiddle");
        UpdateFinger("leftRing");
        UpdateFinger("leftPinky");
    }

    public void UpdateFingerPercentage(string finger)
    {
        int i = 0;

        foreach (var bone in OldHandBones.fingerBones[finger])
        {
            try
            {
                // Debug.Log($"th: {ThumbGrip.Length}, in: {IndexGrip.Length}, mi: {MiddleGrip.Length}, ri: {RingGrip.Length}, pi:{PinkyGrip.Length}");
                if (finger.Contains("Thumb"))
                {
                    if (finger.Contains("left"))
                    {
                        LeftHandBones.Grip(finger, leftThumbGripPercentage);
                    }
                    else
                    {
                        RightHandBones.Grip(finger, rightThumbGripPercentage);
                    }
                }
                else if (finger.Contains("Index"))
                {
                    if (finger.Contains("left"))
                    {
                        LeftHandBones.Grip(finger, leftIndexGripPercentage);
                    }
                    else
                    {
                        RightHandBones.Grip(finger, rightIndexGripPercentage);
                    }
                }
                else if (finger.Contains("Middle"))
                {
                    if (finger.Contains("left"))
                    {
                        LeftHandBones.Grip(finger, leftMiddleGripPercentage);
                    }
                    else
                    {
                        RightHandBones.Grip(finger, rightMiddleGripPercentage);
                    }
                }
                else if (finger.Contains("Ring"))
                {
                    if (finger.Contains("left"))
                    {
                        LeftHandBones.Grip(finger, leftRingGripPercentage);
                    }
                    else
                    {
                        RightHandBones.Grip(finger, rightRingGripPercentage);
                    }
                }
                else if (finger.Contains("Pinky"))
                {
                    if (finger.Contains("left"))
                    {
                        LeftHandBones.Grip(finger, leftPinkyGripPercentage);
                    }
                    else
                    {
                        RightHandBones.Grip(finger, rightPinkyGripPercentage);
                    }
                }
            }
            catch (Exception e) { Debug.Log(e); }
            i++;
        }
    }

    public void UpdateFingerAngles(string finger, Quaternion[] rotations)
    {
        int i = 0;

        foreach (var bone in OldHandBones.fingerBones[finger])
        {
            try
            {
                // Debug.Log($"th: {ThumbGrip.Length}, in: {IndexGrip.Length}, mi: {MiddleGrip.Length}, ri: {RingGrip.Length}, pi:{PinkyGrip.Length}");
                if (finger.Contains("Thumb"))
                {
                    if (finger.Contains("left"))
                    {
                        LeftHandBones.ThumbAngles[i] = rotations[i];
                    }
                    else
                    {
                        RightHandBones.ThumbAngles[i] = rotations[i];
                    }
                }
                else if (finger.Contains("Index"))
                {
                    if (finger.Contains("left"))
                    {
                        LeftHandBones.IndexAngles[i] = rotations[i];
                    }
                    else
                    {
                        RightHandBones.IndexAngles[i] = rotations[i];
                    }
                }
                else if (finger.Contains("Middle"))
                {
                    if (finger.Contains("left"))
                    {
                        LeftHandBones.MiddleAngles[i] = rotations[i];
                    }
                    else
                    {
                        RightHandBones.MiddleAngles[i] = rotations[i];
                    }
                }
                else if (finger.Contains("Ring"))
                {
                    if (finger.Contains("left"))
                    {
                        LeftHandBones.RingAngles[i] = rotations[i];
                    }
                    else
                    {
                        RightHandBones.RingAngles[i] = rotations[i];
                    }
                }
                else if (finger.Contains("Pinky"))
                {
                    if (finger.Contains("left"))
                    {
                        LeftHandBones.PinkyAngles[i] = rotations[i];
                    }
                    else
                    {
                        RightHandBones.PinkyAngles[i] = rotations[i];
                    }
                }
            }
            catch (Exception e) { Debug.Log(e); }
            i++;
        }
    }

    public void UpdateFingerAngles(string finger, float[] angles)
    {
        int i = 0;

        foreach (var bone in OldHandBones.fingerBones[finger])
        {
            try
            {
                // Debug.Log($"th: {ThumbGrip.Length}, in: {IndexGrip.Length}, mi: {MiddleGrip.Length}, ri: {RingGrip.Length}, pi:{PinkyGrip.Length}");
                if (finger.Contains("Thumb"))
                {
                    if (finger.Contains("left"))
                    {
                        LeftHandBones.ThumbAngles[i] = Quaternion.Euler(angles[i], i == 0 ? -90 : 0, -30);
                    }
                    else
                    {
                        RightHandBones.ThumbAngles[i] = Quaternion.Euler(angles[i], i == 0 ? 90 : 0, 30);
                    }
                }
                else if (finger.Contains("Index"))
                {
                    if (finger.Contains("left"))
                    {
                        LeftHandBones.IndexAngles[i] = Quaternion.Euler(angles[i], i == 0 ? -90 : 0, 0);
                    }
                    else
                    {
                        RightHandBones.IndexAngles[i] = Quaternion.Euler(angles[i], i == 0 ? 90 : 0, 0);
                    }
                }
                else if (finger.Contains("Middle"))
                {
                    if (finger.Contains("left"))
                    {
                        LeftHandBones.MiddleAngles[i] = Quaternion.Euler(angles[i], i == 0 ? -90 : 0, 0);
                    }
                    else
                    {
                        RightHandBones.MiddleAngles[i] = Quaternion.Euler(angles[i], i == 0 ? 90 : 0, 0);
                    }
                }
                else if (finger.Contains("Ring"))
                {
                    if (finger.Contains("left"))
                    {
                        LeftHandBones.RingAngles[i] = Quaternion.Euler(angles[i], i == 0 ? -90 : 0, 0);
                    }
                    else
                    {
                        RightHandBones.RingAngles[i] = Quaternion.Euler(angles[i], i == 0 ? 90 : 0, 0);
                    }
                }
                else if (finger.Contains("Pinky"))
                {
                    if (finger.Contains("left"))
                    {
                        LeftHandBones.PinkyAngles[i] = Quaternion.Euler(angles[i], i == 0 ? -90 : 0, 0);
                    }
                    else
                    {
                        RightHandBones.PinkyAngles[i] = Quaternion.Euler(angles[i], i == 0 ? 90 : 0, 0);
                    }
                }
            }
            catch (Exception e) { Debug.Log(e); }
            i++;
        }
    }

    // public void UpdateFinger(string finger)
    // {
    //     int i = 0;
    // 
    //     foreach (var bone in HandBones.fingerBones[finger])
    //     {
    //         try
    //         {
    //             // Debug.Log($"th: {ThumbGrip.Length}, in: {IndexGrip.Length}, mi: {MiddleGrip.Length}, ri: {RingGrip.Length}, pi:{PinkyGrip.Length}");
    //             if (finger.Contains("Thumb"))
    //             {
    //                 if (finger.Contains("left"))
    //                 {
    //                     animator.SetBoneLocalRotation(bone, LeftHandBones.ThumbAngles[i]);
    //                 }
    //                 else
    //                 {
    //                     animator.SetBoneLocalRotation(bone, RightHandBones.ThumbAngles[i]);
    //                 }
    //             }
    //             else if (finger.Contains("Index"))
    //             {
    //                 if (finger.Contains("left"))
    //                 {
    //                     animator.SetBoneLocalRotation(bone, LeftHandBones.IndexAngles[i]);
    //                 }
    //                 else
    //                 {
    //                     animator.SetBoneLocalRotation(bone, RightHandBones.IndexAngles[i]);
    //                 }
    //             }
    //             else if (finger.Contains("Middle"))
    //             {
    //                 if (finger.Contains("left"))
    //                 {
    //                     animator.SetBoneLocalRotation(bone, LeftHandBones.MiddleAngles[i]);
    //                 }
    //                 else
    //                 {
    //                     animator.SetBoneLocalRotation(bone, RightHandBones.MiddleAngles[i]);
    //                 }
    //             }
    //             else if (finger.Contains("Ring"))
    //             {
    //                 if (finger.Contains("left"))
    //                 {
    //                     animator.SetBoneLocalRotation(bone, LeftHandBones.RingAngles[i]);
    //                 }
    //                 else
    //                 {
    //                     animator.SetBoneLocalRotation(bone, RightHandBones.RingAngles[i]);
    //                 }
    //             }
    //             else if (finger.Contains("Pinky"))
    //             {
    //                 if (finger.Contains("left"))
    //                 {
    //                     animator.SetBoneLocalRotation(bone, LeftHandBones.PinkyAngles[i]);
    //                 }
    //                 else
    //                 {
    //                     animator.SetBoneLocalRotation(bone, RightHandBones.PinkyAngles[i]);
    //                 }
    //             }
    //         }
    //         catch (Exception e) { Debug.Log(e); }
    //         i++;
    //     }
    // }

    public void UpdateFinger(string finger)
    {
        int i = 0;

        foreach (var bone in OldHandBones.fingerBones[finger])
        {
            try
            {
                // Debug.Log($"th: {ThumbGrip.Length}, in: {IndexGrip.Length}, mi: {MiddleGrip.Length}, ri: {RingGrip.Length}, pi:{PinkyGrip.Length}");
                if (finger.Contains("Thumb"))
                {
                    if (finger.Contains("left"))
                    {
                        animator.SetBoneLocalRotation(bone, LeftHandBones.ThumbAngles[i]);
                    }
                    else
                    {
                        animator.SetBoneLocalRotation(bone, RightHandBones.ThumbAngles[i]);
                    }
                }
                else if (finger.Contains("Index"))
                {
                    if (finger.Contains("left"))
                    {
                        animator.SetBoneLocalRotation(bone, LeftHandBones.IndexAngles[i]);
                    }
                    else
                    {
                        animator.SetBoneLocalRotation(bone, RightHandBones.IndexAngles[i]);
                    }
                }
                else if (finger.Contains("Middle"))
                {
                    if (finger.Contains("left"))
                    {
                        animator.SetBoneLocalRotation(bone, LeftHandBones.MiddleAngles[i]);
                    }
                    else
                    {
                        animator.SetBoneLocalRotation(bone, RightHandBones.MiddleAngles[i]);
                    }
                }
                else if (finger.Contains("Ring"))
                {
                    if (finger.Contains("left"))
                    {
                        animator.SetBoneLocalRotation(bone, LeftHandBones.RingAngles[i]);
                    }
                    else
                    {
                        animator.SetBoneLocalRotation(bone, RightHandBones.RingAngles[i]);
                    }
                }
                else if (finger.Contains("Pinky"))
                {
                    if (finger.Contains("left"))
                    {
                        animator.SetBoneLocalRotation(bone, LeftHandBones.PinkyAngles[i]);
                    }
                    else
                    {
                        animator.SetBoneLocalRotation(bone, RightHandBones.PinkyAngles[i]);
                    }
                }
            }
            catch (Exception e) { Debug.Log(e); }
            i++;
        }
    }



    // Update is called once per frame
    void Update()
    {
    }
}
