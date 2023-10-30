using MagicLeap.MRTK.DeviceManagement.Input.Hands;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum FingerType
{
    Thumb,
    Index,
    Middle,
    Ring,
    Little,
}

public enum FingerBone
{
    Proximal,
    Intermediate,
    Distal,
}

[Serializable]
public struct Finger
{
    public Handedness handedness;
    public FingerType fingerType;

    public Finger(Handedness handedness, FingerType fingerType)
    {
        this.handedness = handedness;
        this.fingerType = fingerType;
    }
}

public enum RobodyBones
{
    // Left fingers
    LeftThumbProximal,
    LeftThumbIntermediate,
    LeftThumbDistal,
    LeftIndexProximal,
    LeftIndexIntermediate,
    LeftIndexDistal,
    LeftMiddleProximal,
    LeftMiddleIntermediate,
    LeftMiddleDistal,
    LeftRingProximal,
    LeftRingIntermediate,
    LeftRingDistal,
    LeftLittleProximal,
    LeftLittleIntermediate,
    LeftLittleDistal,

    // Right fingers
    RightThumbProximal,
    RightThumbIntermediate,
    RightThumbDistal,
    RightIndexProximal,
    RightIndexIntermediate,
    RightIndexDistal,
    RightMiddleProximal,
    RightMiddleIntermediate,
    RightMiddleDistal,
    RightRingProximal,
    RightRingIntermediate,
    RightRingDistal,
    RightLittleProximal,
    RightLittleIntermediate,
    RightLittleDistal,
}

public class RobodyArmature : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private string ConstructFingerBoneName(Handedness handedness, FingerType fingerType, FingerBone fingerBone)
    {
        string res = "";

        switch (handedness)
        {
            case Handedness.Left:
                res += "lh_";
                break;
            case Handedness.Right:
                res += "rh_";
                break;
        }

        switch (fingerType)
        {
            case FingerType.Thumb:
                res += "THJ";
                break;
            case FingerType.Index:
                res += "FFJ";
                break;
            case FingerType.Middle:
                res += "MFJ";
                break;
            case FingerType.Ring:
                res += "RFJ";
                break;
            case FingerType.Little:
                res += "LFJ";
                break;
        }

        switch (fingerBone)
        {
            case FingerBone.Proximal:
                if (fingerType == FingerType.Thumb)
                {
                    res += "4";
                }
                else
                {
                    res += "3";
                }
                break;
            case FingerBone.Intermediate:
                res += "2";
                break;
            case FingerBone.Distal:
                res += "1";
                break;
        }

        return res;
    }

    public Transform GetFingerBoneTransform(RobodyBones robodyBone)
    {
        Handedness handedness = Handedness.Left;
        FingerType fingerType = FingerType.Thumb;
        FingerBone fingerBone = FingerBone.Proximal;

        string boneName = robodyBone.ToString();
        if (boneName.Contains("Left"))
        {
            handedness = Handedness.Left;
        }
        else if (boneName.Contains("Right"))
        {
            handedness = Handedness.Right;
        }

        if (boneName.Contains("Thumb"))
        {
            fingerType = FingerType.Thumb;
        }
        else if (boneName.Contains("Index"))
        {
            fingerType = FingerType.Index;
        }
        else if (boneName.Contains("Middle"))
        {
            fingerType = FingerType.Middle;
        }
        else if (boneName.Contains("Ring"))
        {
            fingerType = FingerType.Ring;
        }
        else if (boneName.Contains("Little"))
        {
            fingerType = FingerType.Little;
        }

        if (boneName.Contains("Proximal"))
        {
            fingerBone = FingerBone.Proximal;
        }
        else if (boneName.Contains("Intermediate"))
        {
            fingerBone = FingerBone.Intermediate;
        }
        else if (boneName.Contains("Distal"))
        {
            fingerBone = FingerBone.Distal;
        }

        return GameObject.Find(ConstructFingerBoneName(handedness, fingerType, fingerBone)).transform;
    }
}
