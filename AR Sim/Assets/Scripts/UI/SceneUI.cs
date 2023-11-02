using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneUI : MonoBehaviour
{
    public Transform sceneUITarget;

    void Update()
    {
        transform.position = sceneUITarget.position;
        transform.rotation = sceneUITarget.rotation;
    }
}
