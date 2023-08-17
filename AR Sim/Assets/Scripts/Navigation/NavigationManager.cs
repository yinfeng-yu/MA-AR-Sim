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
    /// The NavMeshAgent component attached to the main camera.
    /// </summary>
    private NavMeshAgent _navMeshAgent;

    [SerializeField] private bool m_reachedTarget = true;
    [SerializeField] private Transform m_target;
    [SerializeField] private string m_targetStr;

    [SerializeField] private float _offest = 0.5f;

    bool _shouldPatrol = false;
    Vector2[] _waypoints;
    int currentWaypoint = 0;

    // Start is called before the first frame update
    void Start()
    {
        _navMeshAgent = Camera.main.gameObject.GetComponent<NavMeshAgent>();
        _navMeshAgent.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // if (!m_reachedTarget && Vector3.Distance(_navMeshAgent.gameObject.transform.position, m_target.position) <= _navMeshAgent.stoppingDistance + _offestY)
        // {
        //     _navMeshAgent.transform.forward = m_target.forward;
        //     m_reachedTarget = true;
        //     _navMeshAgent.enabled = false;
        // 
        //     // NotificationManager.instance.NotifyArrived(m_targetStr);
        // }


        if (_shouldPatrol)
        {
            if (_waypoints.Length > currentWaypoint)
            {
                Vector2 waypoint = _waypoints[currentWaypoint];
                Vector3 destination = new Vector3(waypoint.x, 0f, waypoint.y);
                _navMeshAgent.SetDestination(destination);

                if (Vector3.Distance(_navMeshAgent.gameObject.transform.position, destination) <= _navMeshAgent.stoppingDistance + _offest)
                {
                    currentWaypoint++;
                    if (currentWaypoint == _waypoints.Length)
                    {
                        currentWaypoint = 0;
                    }
                }

            }
        }
    }

    void SetDestination(Vector3 destination)
    {

        _navMeshAgent.SetDestination(destination);
    }

    public void SetDestination(Vector2 destination)
    {
        StopPatrol();
        _navMeshAgent.enabled = true;
        
        _navMeshAgent.SetDestination(new Vector3(destination.x, 0f, destination.y));
    }

    public void GoToTarget(Transform a_target, string a_siteEnumStr)
    {
        Debug.Log("Go to target");
        _navMeshAgent.enabled = true;
    
        m_target = a_target;
        SetDestination(a_target.position);
    
        m_targetStr = a_siteEnumStr;
    
        m_reachedTarget = false;
    }

    public void StartPatrol(Vector2[] waypoints)
    {
        _navMeshAgent.enabled = true;
        _shouldPatrol = true;
        _waypoints = waypoints;
    }

    public void StopPatrol()
    {
        _navMeshAgent.enabled = false;
        _shouldPatrol = false;
    }
}
