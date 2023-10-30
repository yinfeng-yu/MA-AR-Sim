using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class RobodyMovement : MonoBehaviour
{
    /// <summary>
    /// Steering speed. Unit per second.
    /// </summary>
    public float steerSpeed = 1f;

    /// <summary>
    /// Rotating speed. Euler Angle per second.
    /// </summary>
    public float rotateSpeed = 30f;

    private bool _isSteering = false;
    private Direction _steerDirection = Direction.None;

    private void Update()
    {
        if (_isSteering)
        {
            switch (_steerDirection)
            {
                case Direction.Forward:
                    transform.position += transform.forward * steerSpeed * Time.deltaTime;
                    break;
                case Direction.Back:
                    transform.position += -transform.forward * steerSpeed * Time.deltaTime;
                    break;
                case Direction.Left:
                    transform.Rotate(0.0f, -rotateSpeed * Time.deltaTime, 0.0f);
                    break;
                case Direction.Right:
                    transform.Rotate(0.0f, rotateSpeed * Time.deltaTime, 0.0f);
                    break;
                default:
                    break;
            }
        }
    }

    public void Steer(Direction direction)
    {
        if (direction == Direction.None)
        {
            _isSteering = false;
        }
        else
        {
            _isSteering = true;

        }
        _steerDirection = direction;
    }
}
