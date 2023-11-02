using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public Transform[] objects;
    public float triggerDistance = 0.4f;
    public GameObject vfx;

    public bool activated = false;

    // Start is called before the first frame update
    void Start()
    {
        Deactivate();
    }

    public void Activate()
    {
        activated = true;
        vfx.SetActive(true);
    }

    void Deactivate()
    {
        activated = false;
        vfx.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
            foreach (var obj in objects)
            {
                if (Vector3.Distance(obj.position, transform.position) <= triggerDistance)
                {
                    ExperimentUtils.Instance.TargetTriggered();
                    Deactivate();
                }
            }
        }
          
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, triggerDistance);
    }
}
