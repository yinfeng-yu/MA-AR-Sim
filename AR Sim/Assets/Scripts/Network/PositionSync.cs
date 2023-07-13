using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionSync : MonoBehaviour
{
    public float updatePeriod = 0.5f;

    private float m_elapsedTime = 0f;

    // Update is called once per frame
    void Update()
    {
        if (m_elapsedTime >= updatePeriod)
        {
            m_elapsedTime = 0f;
            SendPosition();
        }
        else
        {
            m_elapsedTime += Time.deltaTime;
        }
    }


    void SendPosition()
    {
        TransportManager.instance.SendPosition(transform.position);
    }
}
