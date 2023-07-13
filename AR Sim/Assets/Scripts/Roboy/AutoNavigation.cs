using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AutoNavigation : MonoBehaviour
{
    #region Singleton
    public static AutoNavigation instance;
    private void Awake()
    {
        if (instance != this)
        {
            instance = this;
        }
    }
    #endregion

    public Transform target;

    public void GoToLocation(Transform a_target)
    {
        Camera.main.transform.position = new Vector3(a_target.position.x, Camera.main.transform.position.y, a_target.position.z);
        Camera.main.transform.forward = a_target.forward;
    }
}
