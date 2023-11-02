using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediaPipeHandTrackingSkeleton : MonoBehaviour
{
    public Transform[] handPoints;
    public Transform[] handLines;

    // Update is called once per frame
    void Update()
    {
        UpdateLines(handLines, handPoints);
    }

    public void SetPointLocalPosition(int i, Vector3 localPosition)
    {
        handPoints[i].localPosition = localPosition;
    }

    void UpdateLines(Transform[] handLines, Transform[] handPoints)
    {
        for (int i = 0; i < 21; i++)
        {
            // if (handPoints[i].position[0].position == float.NaN) return;

            if (i == 4 || i == 16)
            {
                float length = Vector3.Distance(handPoints[0].position, handPoints[i + 1].position);
                Vector3 forward = (handPoints[i + 1].position - handPoints[0].position).normalized;

                handLines[i].position = handPoints[0].position;
                if (forward != Vector3.zero) handLines[i].forward = forward;
                handLines[i].localScale = new Vector3(length, length, length);
            }

            else if (i == 8 || i == 12)
            {
                float length = Vector3.Distance(handPoints[i - 3].position, handPoints[i + 1].position);
                Vector3 forward = (handPoints[i + 1].position - handPoints[i - 3].position).normalized;

                handLines[i].position = handPoints[i - 3].position;
                if (forward != Vector3.zero) handLines[i].forward = forward;
                handLines[i].localScale = new Vector3(length, length, length);

            }

            else if (i == 20)
            {
                float length = Vector3.Distance(handPoints[13].position, handPoints[17].position);
                Vector3 forward = (handPoints[17].position - handPoints[13].position).normalized;

                handLines[i].position = handPoints[13].position;
                if (forward != Vector3.zero) handLines[i].forward = forward;
                handLines[i].localScale = new Vector3(length, length, length);

            }

            else
            {
                float length = Vector3.Distance(handPoints[i].position, handPoints[i + 1].position);
                Vector3 forward = (handPoints[i + 1].position - handPoints[i].position).normalized;

                handLines[i].position = handPoints[i].position;
                if (forward != Vector3.zero) handLines[i].forward = forward;
                handLines[i].localScale = new Vector3(length, length, length);

            }
        }
    }
}
