using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTracking : MonoBehaviour
{
    [SerializeField] public GameObject[] HandPoints;
    [SerializeField] public GameObject[] HandLines;

    public float Scale = 100;

    public Vector3 handRootTranslation;
    public Vector2 handRootScreenPos;
    public Vector3 palmForward;
    public Vector3 palmNorm;

    public float handDistance;
    public float handDistanceScale = 0.1f;

    public float palmLength = 10f; // in centimeter
    public float focalLength = 680f; // in centimeter

    public bool flipPalm = false;

    float trackedPalmLength;
    Vector3[] handPointsTranslations = new Vector3[21];
    // public Vector3 handRootRotation;

    public float Q = 0.0001f;
    public float R = 0.01f;
    KalmanFilterVector3 kalmanFilter = new KalmanFilterVector3();

    private void Start()
    {
        kalmanFilter = new KalmanFilterVector3(Q, R);
    }

    // Update is called once per frame
    void Update()
    {
        string data = GlobalVariableManager.Instance.HandLandmarksRaw;
        if (data.Length < 1) return;
        
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] == '[')
            {
                trackedPalmLength = float.Parse(data.Substring(0, i));
                data = data.Substring(i, data.Length - i);
            }
        }

        data = data.Substring(1, data.Length - 2);

        string[] points = data.Split(',');

        for (int i = 0; i < 21; i++)
        {
            float x = float.Parse(points[i * 3]) / Scale;
            float y = float.Parse(points[i * 3 + 1]) / Scale;
            float z = float.Parse(points[i * 3 + 2]) / Scale;

            if (i == 0)
            {
                handRootTranslation = new Vector3(x, y, z);
                handRootScreenPos = new Vector2(x * Scale, y * Scale);
            }

            // HandPoints[i].transform.localPosition = new Vector3(x, y, z) - handRootTranslation;
            handPointsTranslations[i] = new Vector3(x, y, z) - handRootTranslation;
            
        }

        Vector3 palmVec1 = handPointsTranslations[5] - handPointsTranslations[0];
        Vector3 palmVec2 = handPointsTranslations[17] - handPointsTranslations[0];

        palmNorm = (Vector3.Cross(palmVec1, palmVec2) * (flipPalm ? -1 : 1)).normalized;
        palmForward = (palmVec1 + palmVec2).normalized;

        // trackedPalmLength = Vector3.Distance(handPointsTranslations[0], handPointsTranslations[5]);
        float handScale = palmLength / trackedPalmLength;

        for (int i = 0; i < 21; i++)
        {
            handPointsTranslations[i] *= handScale;
            HandPoints[i].transform.localPosition = handPointsTranslations[i];
        }

        handRootTranslation *= handScale;

        handRootTranslation = kalmanFilter.Update(handRootTranslation);

        float distance = focalLength * palmLength / trackedPalmLength;
        handDistance = distance * handDistanceScale;

        UpdateLines();
    }

    void UpdateLines()
    {
        for (int i = 0; i < 21; i++)
        {
            if (i == 4 || i == 16)
            {
                float length = Vector3.Distance(handPointsTranslations[0], handPointsTranslations[i + 1]);
                Vector3 forward = (handPointsTranslations[i + 1] - handPointsTranslations[0]).normalized;

                HandLines[i].transform.localPosition = handPointsTranslations[0];
                HandLines[i].transform.forward = forward;
                HandLines[i].transform.localScale = new Vector3(length, length, length);
            }
                
            else if (i == 8 || i == 12)
            {
                float length = Vector3.Distance(handPointsTranslations[i - 3], handPointsTranslations[i + 1]);
                Vector3 forward = (handPointsTranslations[i + 1] - handPointsTranslations[i - 3]).normalized;

                HandLines[i].transform.localPosition = handPointsTranslations[i - 3];
                HandLines[i].transform.forward = forward;
                HandLines[i].transform.localScale = new Vector3(length, length, length);

            }

            else if (i == 20)
            {
                float length = Vector3.Distance(handPointsTranslations[13], handPointsTranslations[17]);
                Vector3 forward = (handPointsTranslations[17] - handPointsTranslations[13]).normalized;

                HandLines[i].transform.localPosition = handPointsTranslations[13];
                HandLines[i].transform.forward = forward;
                HandLines[i].transform.localScale = new Vector3(length, length, length);

            }

            else
            {
                float length = Vector3.Distance(handPointsTranslations[i], handPointsTranslations[i + 1]);
                Vector3 forward = (handPointsTranslations[i + 1] - handPointsTranslations[i]).normalized;

                HandLines[i].transform.localPosition = handPointsTranslations[i];
                HandLines[i].transform.forward = forward;
                HandLines[i].transform.localScale = new Vector3(length, length, length);

            }
        }
    }

    
}
