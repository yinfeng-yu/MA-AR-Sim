using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavigationManager : MonoBehaviour
{
    #region Singleton
    public static NavigationManager instance;
    private void Awake()
    {
        if (instance != this)
        {
            instance = this;
        }
    }
    #endregion

    /// <summary>
    /// The NavMeshAgent component attached to the Roboy.
    /// </summary>
    [SerializeField] private NavMeshAgent _navMeshAgent;

    [SerializeField] private bool _reachedTarget = true;

    [SerializeField] private float _offset = 0.5f;

    bool _shouldPatrol = false;
    Vector2[] _waypoints;
    int currentWaypoint = 0;

    bool hasSentDisplaceNotification = true;

    // Start is called before the first frame update
    void Start()
    {
        _navMeshAgent.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_shouldPatrol && !_reachedTarget && Vector3.Distance(_navMeshAgent.gameObject.transform.position, _navMeshAgent.destination) <= _navMeshAgent.stoppingDistance + _offset)
        {
            _reachedTarget = true;
            _navMeshAgent.enabled = false;

            var roboyPosition = _navMeshAgent.gameObject.transform.localPosition;
            NotificationManager.Instance.SendNotification(TaskType.Displace, TaskStatus.End, (new Vector2(roboyPosition.x, roboyPosition.z)).ToString());
            // NotificationManager.instance.NotifyArrived(m_targetStr);
        }

        if (_shouldPatrol)
        {
            // Debug.Log("patrolling");
            if (_waypoints.Length > currentWaypoint)
            {
                Vector2 waypoint = _waypoints[currentWaypoint];

                Vector3 roboyParentPosition = _navMeshAgent.GetComponentInParent<Transform>().parent.position;
                Vector3 targetPosition = roboyParentPosition + new Vector3(waypoint.x, 0f, waypoint.y);

                // Debug.Log($"target position: {destination}");
                // Debug.Log($"navagent on navmesh?: {_navMeshAgent.isOnNavMesh}");
                bool d = _navMeshAgent.SetDestination(targetPosition);
                // Debug.Log($"successfully set?: {d}");

                if (Vector3.Distance(_navMeshAgent.gameObject.transform.position, targetPosition) <= _navMeshAgent.stoppingDistance + _offset)
                {
                    currentWaypoint++;
                    if (currentWaypoint == _waypoints.Length)
                    {
                        currentWaypoint = 0;
                    }
                }

            }
        }

        TransmissionManager.Instance.SendTo(new Vector3Message("robodyPosition", _navMeshAgent.gameObject.transform.localPosition), Platform.Smartphone);
    }

    void SetDestination(Vector3 destination)
    {
        _navMeshAgent.SetDestination(destination);
    }

    public void SetDestination(Vector2 destination)
    {
        hasSentDisplaceNotification = false;
        _reachedTarget = false;
        // Debug.Log($"Roboy Parent Name: {_navMeshAgent.GetComponentInParent<Transform>().parent.name}");
        Vector3 roboyParentPosition = _navMeshAgent.GetComponentInParent<Transform>().parent.position;
        Vector3 targetPosition = roboyParentPosition + new Vector3(destination.x, 0f, destination.y);

        if (_shouldPatrol)
        {
            StopPatrol();
        }
        
        _navMeshAgent.enabled = true;

        bool d = _navMeshAgent.SetDestination(targetPosition);

        NotificationManager.Instance.SendNotification(TaskType.Displace, TaskStatus.Start, "");
        // Debug.Log($"Successfully set? {d}");

        // Debug.Log($"Set to: {targetPosition}");
    }

    // public void GoToTarget(Transform a_target, string a_siteEnumStr)
    // {
    //     Debug.Log("Go to target");
    //     _navMeshAgent.enabled = true;
    // 
    //     m_target = a_target;
    //     SetDestination(a_target.position);
    // 
    //     m_targetStr = a_siteEnumStr;
    // 
    //     m_reachedTarget = false;
    // }

    public void StartPatrol(Vector2[] waypoints)
    {
        _navMeshAgent.enabled = true;
        _shouldPatrol = true;
        _waypoints = waypoints;

        NotificationManager.Instance.SendNotification(TaskType.Patrol, TaskStatus.Start, "");
    }

    public void StopPatrol()
    {
        _navMeshAgent.enabled = false;
        _shouldPatrol = false;

        NotificationManager.Instance.SendNotification(TaskType.Patrol, TaskStatus.End, "");
    }
}
