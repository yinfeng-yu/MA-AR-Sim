using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robody : Singleton<Robody>
{
    public Transform RobodyTransform;
    public Vector3 position;
    public RobodyMovement movement;

    // Start is called before the first frame update
    void Start()
    {
        movement = FindObjectOfType<RobodyMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        position = transform.position;
    }
}
