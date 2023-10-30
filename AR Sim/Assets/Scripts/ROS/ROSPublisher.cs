using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
// using RosMessageTypes.UnityRoboticsDemo;

using RosMessageTypes.Std;
using RosMessageTypes.Sensor;
using UnityEngine.UIElements;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public class ROSPublisher : MonoBehaviour
{
    ROSConnection ros;
    [SerializeField] private string topicName = "/roboy/pinky/control/joint_targets";

    [SerializeField] private string[] jointNames = new[] { "shoulder_left_axis0", "shoulder_left_axis1", "shoulder_left_axis2", "elbow_left_axis0", "elbow_left_axis1", "wrist_left_axis0", "wrist_left_axis1", "wrist_left_axis2" };
    // public string jointName = "shoulder_left_axis0";

    public bool isPublish = false;
    public bool debugPublish = false;
    public bool debugSubscribe = false;

    public bool isTest = true;

    // The game object 
    public GameObject robody;

    // Publish the cube's position and rotation every N seconds
    public float publishMessageFrequency = 0.5f;

    [Header("Publish Test")]
    [Range(-1, 1)]
    public float shoulder_axis0;
    [Range(-1, 1)]
    public float shoulder_axis1;
    [Range(-1, 1)]
    public float shoulder_axis2;
    [Range(-1, 1)]
    public float elbow_axis0;
    [Range(-1, 1)]
    public float elbow_axis1;
    [Range(-1, 1)]
    public float wrist_axis0;
    [Range(-1, 1)]
    public float wrist_axis1;
    [Range(-1, 1)]
    public float wrist_axis2;

    private List<float> publishArray = new List<float>();

    // Used to determine how much time has elapsed since the last message was published
    private float _timeElapsed;

    private Dictionary<string, Transform> _joints = new Dictionary<string, Transform>();

    void Start()
    {
        for (int i = 0; i < jointNames.Length; i++)
        {
            publishArray.Add(0);
        }

        // Start the ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<JointStateMsg>(topicName);

        // Initialize the joints list
        foreach (var jointName in jointNames)
        {
            Transform jointTransform = GameObject.Find(jointName).transform;
            _joints[jointName] = jointTransform;
            
        }
    }

    private void Update()
    {
        publishArray[0] = shoulder_axis0;
        publishArray[1] = shoulder_axis1;
        publishArray[2] = shoulder_axis2;
        publishArray[3] = elbow_axis0;
        publishArray[4] = elbow_axis1;
        publishArray[5] = wrist_axis0;
        publishArray[6] = wrist_axis1;
        publishArray[7] = wrist_axis2;

        if (isPublish)
        {
            _timeElapsed += Time.deltaTime;

            if (_timeElapsed > publishMessageFrequency)
            {
                int i = 0;
                foreach (var jointName in jointNames)
                {
                    if (isTest)
                    {
                        Transform joint = _joints[jointName];
                        JointStateMsg jointState = new JointStateMsg(
                            new HeaderMsg(),
                            new[] { jointName }, // joint name
                                                 // new[] { (double)joint.position[0], joint.position[1], joint.position[2] }, // position
                            new[] { (double)publishArray[i] }, // position
                            new double[0], // velocity
                            new double[0] // effort
                        );

                        // Finally send the message to server_endpoint.py running in ROS
                        ros.Publish(topicName, jointState);

                        if (debugPublish)
                        {
                            Debug.Log($"Publish [name:{jointState.name[0]}, position:({jointState.position[0]}, {jointState.position[1]}, {jointState.position[2]})] to {topicName}");
                        }
                    }
                    else
                    {
                        Transform joint = _joints[jointName];
                        IKJoint ikJoint = joint.gameObject.GetComponent<IKJoint>();
                        if (ikJoint == null)
                        {
                            continue;
                        }

                        JointStateMsg jointState = new JointStateMsg(
                            new HeaderMsg(),
                            new[] { jointName }, // joint name
                                                 // new[] { (double)joint.position[0], joint.position[1], joint.position[2] }, // position
                            new[] { (double)ikJoint.CalculatePublishPos(joint.localEulerAngles[(int)ikJoint.axis]) }, // position
                            new double[0], // velocity
                            new double[0] // effort
                        );

                        // Finally send the message to server_endpoint.py running in ROS
                        ros.Publish(topicName, jointState);

                        if (debugPublish)
                        {
                            Debug.Log($"Publish [name:{jointState.name[0]}, position: {jointState.position[0]}] to {topicName}");
                        }
                    }

                    i++;
                }


                _timeElapsed = 0;
            }
        }
        
    }

    float GetShoulderLeftAxis0()
    {
        Transform shoulderLeftAxis0 = _joints["shoulder_left_axis0"];
        float angle = shoulderLeftAxis0.localEulerAngles.z;
        if (angle > 180)
        {
            angle -= 360;        
        }

        float clamped = Mathf.Clamp(angle, -140, -40);
        // Debug.Log($"shoulder 0: original angle = {angle}, clamped angle = {clamped}, lerp = {Mathf.Lerp(1, -1, (clamped + 140) / 100)}");
        return Mathf.Lerp(1, -1, (clamped + 140) / 100); // -140 => 1; -90 => 0; -40 => -1
    }

    float GetShoulderLeftAxis1()
    {
        Transform shoulderLeftAxis0 = _joints["shoulder_left_axis0"];
        float angle = shoulderLeftAxis0.localEulerAngles.y;
        if (angle > 180)
        {
            angle -= 360;
        }

        float clamped = Mathf.Clamp(angle, -140, -40);
        // Debug.Log($"shoulder 1: original angle = {angle}, clamped angle = {clamped}, lerp = {Mathf.Lerp(1, -1, (clamped + 140) / 100)}");
        return Mathf.Lerp(1, -1, (clamped + 140) / 100); // -140 => 1; -90 => 0; -40 => -1
    }

    float GetElbowLeftAxis0()
    {
        Transform elbowLeftAxis0 = _joints["elbow_left_axis0"];
        float angle = elbowLeftAxis0.localEulerAngles.z;
        if (angle > 180)
        {
            angle -= 360;
        }
        float clamped = Mathf.Clamp(angle, 0, 45);

        // Debug.Log($"elbow 0: original angle = {angle}, clamped angle = {clamped}, lerp = {Mathf.Lerp(0, 1, clamped / 45)}");
        return Mathf.Lerp(0, 1, clamped / 45); // 0 => 0; 45 => 1
    }

    float GetElbowLeftAxis1()
    {
        Transform elbowLeftAxis0 = _joints["elbow_left_axis0"];
        float angle = elbowLeftAxis0.localEulerAngles.z;
        if (angle > 180)
        {
            angle -= 360;
        }
        float clamped = Mathf.Clamp(angle, 0, 90);
        if (clamped > 45)
        {
            clamped -= 45;
        }

        // Debug.Log($"elbow 1: original angle = {angle}, clamped angle = {clamped}, lerp = {Mathf.Lerp(0, 1, clamped / 45)}");
        return Mathf.Lerp(0, 1, clamped / 45); // 0 => 0; 45 => 1
    }
}