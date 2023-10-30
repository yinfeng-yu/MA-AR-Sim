using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKJoint : MonoBehaviour
{
    public enum Axis
    {
        X = 0, 
        Y, 
        Z
    }

    public Axis axis;

    [Header("Constraints")]
    public float minAngle;
    public float maxAngle;

    public float minROSAngle;
    public float maxROSAngle;

    public float minPublishPos;
    public float maxPublishPos;

    [SerializeField] private IKJoint _child;

    private float _minAngle;
    private float _maxAngle;

    public IKJoint GetChild()
    {
        return _child;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="angle"> Ranged between 0 and 1 </param>
    public void Rotate(float angle, bool isROSConstrained)
    {
        var eulers = new Vector3();
        eulers[(int)axis] = angle;
        
        transform.Rotate(eulers);

        if (isROSConstrained)
        {
            _minAngle = minROSAngle;
            _maxAngle = maxROSAngle;
        }
        else
        {
            _minAngle = minAngle;
            _maxAngle = maxAngle;
        }

        float rotatedAngle = transform.localEulerAngles[(int)axis];

        if (rotatedAngle > 180)
        {
            rotatedAngle -= 360;
        }

        if (rotatedAngle < _minAngle)
        {
            eulers[(int)axis] = _minAngle - rotatedAngle;
            transform.Rotate(eulers);
        }
        else if (rotatedAngle > _maxAngle)
        {
            eulers[(int)axis] = rotatedAngle - _maxAngle;
            transform.Rotate(-eulers);
        }
    }

    public float CalculatePublishPos(float angle)
    {
        if (angle > 180)
        {
            angle -= 360;
        }
        angle = Mathf.Clamp(angle, _minAngle, _maxAngle);
        return Mathf.Lerp(minPublishPos, maxPublishPos, (angle - _minAngle) / (_maxAngle - _minAngle));
    }

}
