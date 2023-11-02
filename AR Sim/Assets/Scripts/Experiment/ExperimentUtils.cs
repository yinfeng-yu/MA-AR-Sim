using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ExperimentTask
{
    Move,
    Switch,
    Grab,
}

public class ExperimentUtils : Singleton<ExperimentUtils>
{
    public GameObject robody;

    public Transform robodyOrigin;

    public Transform[] grabbables;

    private Vector3[] _grabbablesOriginPos;
    private Quaternion[] _grabbablesOriginRot;

    public float elapsedTime;

    public ExperimentTask task;

    public bool isTaskRunning = false;

    public Target moveTarget;
    public Target grabTarget;
    public LightSwitchController lightSwitchController;

    // Start is called before the first frame update
    void Start()
    {
        _grabbablesOriginPos = new Vector3[grabbables.Length];
        _grabbablesOriginRot = new Quaternion[grabbables.Length];

        for (int i = 0; i < grabbables.Length; i++)
        {
            _grabbablesOriginPos[i] = grabbables[i].position;
            _grabbablesOriginRot[i] = grabbables[i].rotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isTaskRunning)
        {
            elapsedTime += Time.deltaTime;

            if (task == ExperimentTask.Switch && !lightSwitchController.lightsOn)
            {
                TargetTriggered();
            }
        }
    }

    public void ResetScene()
    {
        robody.transform.position = robodyOrigin.position;
        for (int i = 0; i < grabbables.Length; i++)
        {
            grabbables[i].position = _grabbablesOriginPos[i];
            grabbables[i].rotation = _grabbablesOriginRot[i];
        }

        if (!lightSwitchController.lightsOn)
        {
            lightSwitchController.SwitchLight();
        }

        elapsedTime = 0;
    }

    public void StartMoveTask()
    {
        elapsedTime = 0;
        task = ExperimentTask.Move;
        isTaskRunning = true;
        moveTarget.Activate();
    }

    public void TargetTriggered()
    {
        isTaskRunning = false;
        Debug.Log($"{task} task is done! Time taken: {elapsedTime}");
    }

    public void StartGrabTask()
    {
        elapsedTime = 0;
        task = ExperimentTask.Grab;
        isTaskRunning = true;
        grabTarget.Activate();
    }

    public void StartSwitchTask()
    {
        elapsedTime = 0;
        task = ExperimentTask.Switch;
        isTaskRunning = true;
    }
}
