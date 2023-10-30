using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShouldJointStateGetter : MonoBehaviour
{
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        var rotation = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm).localRotation;
        var angles = rotation.eulerAngles;
        for (int i = 0; i < 3;  i++)
        {
            if (angles[i] > 180)
            {
                angles[i] -= 360;
            }
        }
        // Debug.Log($"left shoulder rotation: {rotation}, angles: {angles}");
    }
}
