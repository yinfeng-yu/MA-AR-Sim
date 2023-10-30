using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeCursor : MonoBehaviour
{
    public GazeSelector gazeSelector;

    // Update is called once per frame
    void Update()
    {
        transform.position = gazeSelector.gazePoint;
    }
}
