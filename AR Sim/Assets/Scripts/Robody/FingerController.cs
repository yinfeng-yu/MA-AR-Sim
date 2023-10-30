using MagicLeap.MRTK.DeviceManagement.Input.Hands;
using Microsoft.MixedReality.Toolkit.Utilities;
using RosMessageTypes.Std;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FingerController : Singleton<FingerController>
{
    public Quaternion[] ThumbDefault = new[] { Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 0) };
    public Quaternion[] IndexDefault = new[] { Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 0) };
    public Quaternion[] MiddleDefault = new[] { Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 0) };
    public Quaternion[] RingDefault = new[] { Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 0) };
    public Quaternion[] LittleDefault = new[] { Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 0) };

    public Quaternion[] LeftThumbGrip = new[] { Quaternion.Euler(0, 0, 50), Quaternion.Euler(0, -50, 70), Quaternion.Euler(0, 0, 60) };
    public Quaternion[] RightThumbGrip = new[] { Quaternion.Euler(0, 0, -50), Quaternion.Euler(0, 50, -70), Quaternion.Euler(0, 0, -60) };

    public Quaternion[] IndexGrip = new[] { Quaternion.Euler(0, 0, -80), Quaternion.Euler(0, 0, -80), Quaternion.Euler(0, 0, -60) };
    public Quaternion[] MiddleGrip = new[] { Quaternion.Euler(0, 0, -80), Quaternion.Euler(0, 0, -80), Quaternion.Euler(0, 0, -60) };
    public Quaternion[] RingGrip = new[] { Quaternion.Euler(0, 0, -80), Quaternion.Euler(0, 0, -80), Quaternion.Euler(0, 0, -60) };
    public Quaternion[] LittleGrip = new[] { Quaternion.Euler(0, 0, -80), Quaternion.Euler(0, 0, -80), Quaternion.Euler(0, 0, -60) };

    public float[] leftThumbAngles = new[]  { 0f, 0f, 0f };
    public float[] leftIndexAngles = new[]  { 0f, 0f, 0f };
    public float[] leftMiddleAngles = new[] { 0f, 0f, 0f };
    public float[] leftRingAngles = new[]   { 0f, 0f, 0f };
    public float[] leftLittleAngles = new[] { 0f, 0f, 0f };
           
    public float[] rightThumbAngles = new[]  { 0f, 0f, 0f };
    public float[] rightIndexAngles = new[]  { 0f, 0f, 0f };
    public float[] rightMiddleAngles = new[] { 0f, 0f, 0f };
    public float[] rightRingAngles = new[]   { 0f, 0f, 0f };
    public float[] rightLittleAngles = new[] { 0f, 0f, 0f };

    public static Dictionary<Finger, RobodyBones[]> FingerBones = new Dictionary<Finger, RobodyBones[]>()
    {
        { new Finger(Handedness.Right, FingerType.Thumb),  new []{ RobodyBones.RightThumbProximal,  RobodyBones.RightThumbIntermediate,  RobodyBones.RightThumbDistal }},
        { new Finger(Handedness.Right, FingerType.Index),  new []{ RobodyBones.RightIndexProximal,  RobodyBones.RightIndexIntermediate,  RobodyBones.RightIndexDistal }},
        { new Finger(Handedness.Right, FingerType.Middle), new []{ RobodyBones.RightMiddleProximal, RobodyBones.RightMiddleIntermediate, RobodyBones.RightMiddleDistal }},
        { new Finger(Handedness.Right, FingerType.Ring),   new []{ RobodyBones.RightRingProximal,   RobodyBones.RightRingIntermediate,   RobodyBones.RightRingDistal }},
        { new Finger(Handedness.Right, FingerType.Little),  new []{ RobodyBones.RightLittleProximal, RobodyBones.RightLittleIntermediate, RobodyBones.RightLittleDistal }},

        { new Finger(Handedness.Left, FingerType.Thumb),  new []{ RobodyBones.LeftThumbProximal,  RobodyBones.LeftThumbIntermediate,  RobodyBones.LeftThumbDistal }},
        { new Finger(Handedness.Left, FingerType.Index),  new []{ RobodyBones.LeftIndexProximal,  RobodyBones.LeftIndexIntermediate,  RobodyBones.LeftIndexDistal }},
        { new Finger(Handedness.Left, FingerType.Middle), new []{ RobodyBones.LeftMiddleProximal, RobodyBones.LeftMiddleIntermediate, RobodyBones.LeftMiddleDistal }},
        { new Finger(Handedness.Left, FingerType.Ring),   new []{ RobodyBones.LeftRingProximal,   RobodyBones.LeftRingIntermediate,   RobodyBones.LeftRingDistal }},
        { new Finger(Handedness.Left, FingerType.Little),  new []{ RobodyBones.LeftLittleProximal, RobodyBones.LeftLittleIntermediate, RobodyBones.LeftLittleDistal }},
    };

    [Range(0, 1)]
    public float leftGrip;
    [Range(0, 1)]
    public float rightGrip;

    public bool isLeftGrab = false;
    public bool isRightGrab = false;

    public float leftGripPercentage = 0f;
    public float rightGripPercentage = 0f;

    public Transform leftGripPivot;
    public Transform rightGripPivot;

    public float fingerAngleLerpDuration = 0.3f;

    public float grabTime = 0.5f;
    private float _leftGrabTime = 0;
    private float _rightGrabTime = 0;

    // [Header("Left Fingers Percentage")]
    // [Range(0, 1)]
    // public float leftThumbGripPercentage;
    // [Range(0, 1)]
    // public float leftIndexGripPercentage;
    // [Range(0, 1)]
    // public float leftMiddleGripPercentage;
    // [Range(0, 1)]
    // public float leftRingGripPercentage;
    // [Range(0, 1)]
    // public float leftLittleGripPercentage;
    // 
    // [Header("Right Fingers Percentage")]
    // [Range(0, 1)]
    // public float rightThumbGripPercentage;
    // [Range(0, 1)]
    // public float rightIndexGripPercentage;
    // [Range(0, 1)]
    // public float rightMiddleGripPercentage;
    // [Range(0, 1)]
    // public float rightRingGripPercentage;
    // [Range(0, 1)]
    // public float rightLittleGripPercentage;

    private Dictionary<Finger, float> fingerPercentages = new Dictionary<Finger, float>()
    {
        { new Finger(Handedness.Right, FingerType.Thumb),  0f },
        { new Finger(Handedness.Right, FingerType.Index),  0f },
        { new Finger(Handedness.Right, FingerType.Middle), 0f },
        { new Finger(Handedness.Right, FingerType.Ring),   0f },
        { new Finger(Handedness.Right, FingerType.Little), 0f },

        { new Finger(Handedness.Left, FingerType.Thumb),  0f },
        { new Finger(Handedness.Left, FingerType.Index),  0f },
        { new Finger(Handedness.Left, FingerType.Middle), 0f },
        { new Finger(Handedness.Left, FingerType.Ring),   0f },
        { new Finger(Handedness.Left, FingerType.Little), 0f },
    };
    private Dictionary<Finger, float[]> fingerAngles = new Dictionary<Finger, float[]>()
    {
        { new Finger(Handedness.Right, FingerType.Thumb),  new[]  { 0f, 0f, 0f } },
        { new Finger(Handedness.Right, FingerType.Index),  new[]  { 0f, 0f, 0f } },
        { new Finger(Handedness.Right, FingerType.Middle), new[]  { 0f, 0f, 0f } },
        { new Finger(Handedness.Right, FingerType.Ring),   new[]  { 0f, 0f, 0f } },
        { new Finger(Handedness.Right, FingerType.Little), new[]  { 0f, 0f, 0f } },

        { new Finger(Handedness.Left, FingerType.Thumb),  new[]  { 0f, 0f, 0f } },
        { new Finger(Handedness.Left, FingerType.Index),  new[]  { 0f, 0f, 0f } },
        { new Finger(Handedness.Left, FingerType.Middle), new[]  { 0f, 0f, 0f } },
        { new Finger(Handedness.Left, FingerType.Ring),   new[]  { 0f, 0f, 0f } },
        { new Finger(Handedness.Left, FingerType.Little), new[]  { 0f, 0f, 0f } },
    };

    private Dictionary<Finger, float> fingerAngleVelocities = new Dictionary<Finger, float>()
    {
        { new Finger(Handedness.Right, FingerType.Thumb),  0f },
        { new Finger(Handedness.Right, FingerType.Index),  0f },
        { new Finger(Handedness.Right, FingerType.Middle), 0f },
        { new Finger(Handedness.Right, FingerType.Ring),   0f },
        { new Finger(Handedness.Right, FingerType.Little), 0f },

        { new Finger(Handedness.Left, FingerType.Thumb),  0f },
        { new Finger(Handedness.Left, FingerType.Index),  0f },
        { new Finger(Handedness.Left, FingerType.Middle), 0f },
        { new Finger(Handedness.Left, FingerType.Ring),   0f },
        { new Finger(Handedness.Left, FingerType.Little), 0f },
    };

    private Dictionary<Finger, float> fingerGripAngles = new Dictionary<Finger, float>()
    {
        { new Finger(Handedness.Right, FingerType.Thumb),  50f },
        { new Finger(Handedness.Right, FingerType.Index),  60f },
        { new Finger(Handedness.Right, FingerType.Middle), 60f },
        { new Finger(Handedness.Right, FingerType.Ring),   60f },
        { new Finger(Handedness.Right, FingerType.Little), 60f },

        { new Finger(Handedness.Left, FingerType.Thumb),  50f },
        { new Finger(Handedness.Left, FingerType.Index),  60f },
        { new Finger(Handedness.Left, FingerType.Middle), 60f },
        { new Finger(Handedness.Left, FingerType.Ring),   60f },
        { new Finger(Handedness.Left, FingerType.Little), 60f },
    };

    private RobodyArmature _robodyArmature;

    [Range(0, 1)]
    public float grabThreshold = 0.6f;

    // Start is called before the first frame update
    void Start()
    {
        _robodyArmature = FindObjectOfType<RobodyArmature>();

    }

    // Update is called once per frame
    void Update()
    {
        if (ControlModeManager.Instance.currentControlMode == ControlMode.HandTracking)
        {
            UpdatePercentages();
            UpdateGrip();
            int leftGrip = 0;
            int rightGrip = 0;

            foreach (var keyValue in fingerPercentages)
            {
                if (keyValue.Value > grabThreshold)
                {
                    switch (keyValue.Key.handedness) 
                    {
                        case Handedness.Left:
                            leftGrip++;
                            break;
                        case Handedness.Right:
                            rightGrip++; 
                            break;
                    }
                }
            }

            if (leftGrip >= 4)
            {
                isLeftGrab = true;
            }
            else
            {
                isLeftGrab = false;
            }

            if (rightGrip >= 4)
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

                leftGripPercentage = Mathf.Lerp(0, 1, _leftGrabTime / grabTime);

                Grip(Handedness.Left, leftGripPercentage);
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

                leftGripPercentage = Mathf.Lerp(0, 1, _leftGrabTime / grabTime);

                Grip(Handedness.Left, leftGripPercentage);
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

                rightGripPercentage = Mathf.Lerp(0, 1, _rightGrabTime / grabTime);

                Grip(Handedness.Right, rightGripPercentage);
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

                rightGripPercentage = Mathf.Lerp(0, 1, _rightGrabTime / grabTime);

                Grip(Handedness.Right, rightGripPercentage);
            }
        }
    }

    private void UpdateGrip()
    {
        foreach (Finger finger in FingerBones.Keys)
        {
            Grip(finger, fingerPercentages[finger]);
        }
    }

    public void SetFingerGripPercentage(Finger finger, float percentage)
    {
        fingerPercentages[finger] = percentage;
    }

    private void Grip(Finger finger, float percentage)
    {
        int i = 0;
        foreach (var bone in FingerBones[finger])
        {
            try
            {
                Transform fingerBone = _robodyArmature.GetFingerBoneTransform(bone);
                switch (finger.fingerType)
                {
                    case FingerType.Thumb:
                        if (finger.handedness == Handedness.Left)
                        {

                            fingerBone.localRotation = Quaternion.Lerp(ThumbDefault[i], LeftThumbGrip[i], percentage);
                        }
                        else
                        {

                            fingerBone.localRotation = Quaternion.Lerp(ThumbDefault[i], RightThumbGrip[i], percentage);
                        }
                        break;
                    case FingerType.Index:
                        fingerBone.localRotation = Quaternion.Lerp(IndexDefault[i], IndexGrip[i], percentage);
                        break;
                    case FingerType.Middle:
                        fingerBone.localRotation = Quaternion.Lerp(MiddleDefault[i], MiddleGrip[i], percentage);
                        break;
                    case FingerType.Ring:
                        fingerBone.localRotation = Quaternion.Lerp(RingDefault[i], RingGrip[i], percentage);
                        break;
                    case FingerType.Little:
                        fingerBone.localRotation = Quaternion.Lerp(LittleDefault[i], LittleGrip[i], percentage);
                        break;
                }
            }
            catch (Exception e) { /* Debug.Log(e); */ }
            i++;
        }
    }

    private void Grip(Handedness handedness, float percentage)
    {
        if (handedness == Handedness.Left)
        {
            Grip(new Finger(Handedness.Left, FingerType.Thumb), percentage);
            Grip(new Finger(Handedness.Left, FingerType.Index), percentage);
            Grip(new Finger(Handedness.Left, FingerType.Middle), percentage);
            Grip(new Finger(Handedness.Left, FingerType.Ring), percentage);
            Grip(new Finger(Handedness.Left, FingerType.Little), percentage);
        }
        else if (handedness == Handedness.Right)
        {
            Grip(new Finger(Handedness.Right, FingerType.Thumb), percentage);
            Grip(new Finger(Handedness.Right, FingerType.Index), percentage);
            Grip(new Finger(Handedness.Right, FingerType.Middle), percentage);
            Grip(new Finger(Handedness.Right, FingerType.Ring), percentage);
            Grip(new Finger(Handedness.Right, FingerType.Little), percentage);
        }
    }

    public void UpdateFingerAngles(Finger finger, float[] angles)
    {
        fingerAngles[finger] = angles;
        // int i = 0;
        // 
        // foreach (var bone in FingerBones[finger])
        // {
        //     try
        //     {
        //         // Debug.Log($"th: {ThumbGrip.Length}, in: {IndexGrip.Length}, mi: {MiddleGrip.Length}, ri: {RingGrip.Length}, pi:{PinkyGrip.Length}");
        //         if (finger.fingerType == FingerType.Thumb)
        //         {
        //             if (finger.Contains("left"))
        //             {
        //                 leftThumbAngles[i] = Quaternion.Euler(angles[i], i == 0 ? -90 : 0, -30);
        //             }
        //             else
        //             {
        //                 RightHandBones.ThumbAngles[i] = Quaternion.Euler(angles[i], i == 0 ? 90 : 0, 30);
        //             }
        //         }
        //         else if (finger.Contains("Index"))
        //         {
        //             if (finger.Contains("left"))
        //             {
        //                 LeftHandBones.IndexAngles[i] = Quaternion.Euler(angles[i], i == 0 ? -90 : 0, 0);
        //             }
        //             else
        //             {
        //                 RightHandBones.IndexAngles[i] = Quaternion.Euler(angles[i], i == 0 ? 90 : 0, 0);
        //             }
        //         }
        //         else if (finger.Contains("Middle"))
        //         {
        //             if (finger.Contains("left"))
        //             {
        //                 LeftHandBones.MiddleAngles[i] = Quaternion.Euler(angles[i], i == 0 ? -90 : 0, 0);
        //             }
        //             else
        //             {
        //                 RightHandBones.MiddleAngles[i] = Quaternion.Euler(angles[i], i == 0 ? 90 : 0, 0);
        //             }
        //         }
        //         else if (finger.Contains("Ring"))
        //         {
        //             if (finger.Contains("left"))
        //             {
        //                 LeftHandBones.RingAngles[i] = Quaternion.Euler(angles[i], i == 0 ? -90 : 0, 0);
        //             }
        //             else
        //             {
        //                 RightHandBones.RingAngles[i] = Quaternion.Euler(angles[i], i == 0 ? 90 : 0, 0);
        //             }
        //         }
        //         else if (finger.Contains("Pinky"))
        //         {
        //             if (finger.Contains("left"))
        //             {
        //                 LeftHandBones.PinkyAngles[i] = Quaternion.Euler(angles[i], i == 0 ? -90 : 0, 0);
        //             }
        //             else
        //             {
        //                 RightHandBones.PinkyAngles[i] = Quaternion.Euler(angles[i], i == 0 ? 90 : 0, 0);
        //             }
        //         }
        //     }
        //     catch (Exception e) { Debug.Log(e); }
        //     i++;
        // }
    }

    private void UpdatePercentages()
    {
        foreach (var keyValue in fingerAngles)
        {
            var finger = keyValue.Key;
            var angle = keyValue.Value;
            SetFingerPercentage(finger, angle[1]);
        }
    }

    // void SetFingerPercentage(ref float leftPercentage, ref float rightPercentage, Handedness handedness, float angle, float min, float max, ref float leftVelocity, ref float rightVelocity)
    // {
    //     if (handedness == Handedness.Left)
    //     {
    //         var current = leftPercentage;
    //         leftPercentage = Mathf.SmoothDamp(
    //             current,
    //             ((Mathf.Clamp(angle, min, max) - min) / (max - min)),
    //             ref leftVelocity,
    //             fingerAngleLerpDuration);
    //     }
    //     else if (handedness == Handedness.Right)
    //     {
    //         var current = rightPercentage;
    //         rightPercentage = Mathf.SmoothDamp(
    //             current,
    //             -((Mathf.Clamp(angle, -max, min) + min) / (max - min)),
    //             ref rightVelocity,
    //             fingerAngleLerpDuration);
    //     }
    // }

    void SetFingerPercentage(Finger finger, float angle)
    {
        var angleVelocity = fingerAngleVelocities[finger];
        fingerPercentages[finger] = Mathf.SmoothDamp(fingerPercentages[finger],
                                                     ((Mathf.Clamp(angle, 0, fingerGripAngles[finger])) / (fingerGripAngles[finger])),
                                                     ref angleVelocity,
                                                     fingerAngleLerpDuration);
        fingerAngleVelocities[finger] = angleVelocity;

        // switch (finger.fingerType)
        // {
        //     case FingerType.Thumb:
        //         var angleVelocity = fingerAngleVelocities[finger];
        //         fingerPercentages[finger] = Mathf.SmoothDamp(fingerPercentages[finger],
        //                                                      ((Mathf.Clamp(angle, 0, fingerGripAngles[finger])) / (fingerGripAngles[finger])),
        //                                                      ref angleVelocity,
        //                                                      fingerAngleLerpDuration);
        //         fingerAngleVelocities[finger] = angleVelocity;
        //         if (handedness == Handedness.Left)
        //         {
        //             FingerController.Instance.leftThumbGripPercentage = handIK.leftIndexGripPercentage; // ((Mathf.Clamp(leftAngle, minThumb, maxThumb) - minThumb) / (maxThumb - minThumb));
        //         }
        //         else
        //         {
        //             handIK.rightThumbGripPercentage = handIK.rightIndexGripPercentage;
        //         }
        //         break;
        //     case FingerType.Index:
        //         SetFingerPercentage(ref fingerPercentages[finger],
        //                             ref handIK.rightIndexGripPercentage,
        //                             handedness,
        //                             angle,
        //                             minIndex,
        //                             maxIndex,
        //                             ref leftIndexAngleVelocity,
        //                             ref rightIndexAngleVelocity);
        //         break;
        //     case FingerType.Middle:
        //         SetFingerPercentage(ref handIK.leftMiddleGripPercentage,
        //                              ref handIK.rightMiddleGripPercentage,
        //                              handedness,
        //                              angle,
        //                              minMiddle,
        //                              maxMiddle,
        //                              ref leftMiddleAngleVelocity,
        //                              ref rightMiddleAngleVelocity);
        //         break;
        //     case FingerType.Ring:
        //         SetFingerPercentage(ref handIK.leftRingGripPercentage,
        //                              ref handIK.rightRingGripPercentage,
        //                              handedness,
        //                              angle,
        //                              minRing,
        //                              maxRing,
        //                              ref leftRingAngleVelocity,
        //                              ref rightRingAngleVelocity);
        //         break;
        //     case FingerType.Little:
        //         SetFingerPercentage(ref handIK.leftPinkyGripPercentage,
        //                              ref handIK.rightPinkyGripPercentage,
        //                              handedness,
        //                              angle,
        //                              minPinky,
        //                              maxPinky,
        //                              ref leftPinkyAngleVelocity,
        //                              ref rightPinkyAngleVelocity);
        //         break;
        //     default:
        //         break;
        // }
    }
}
