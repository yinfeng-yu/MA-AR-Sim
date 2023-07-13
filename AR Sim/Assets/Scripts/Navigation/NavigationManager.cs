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
    private NavMeshAgent m_navMeshAgent;

    [SerializeField] private bool m_reachedTarget = true;
    [SerializeField] private Transform m_target;
    [SerializeField] private string m_targetStr;

    [SerializeField] private float m_offestY = 1.4f;

    // Start is called before the first frame update
    void Start()
    {
        m_navMeshAgent = Camera.main.gameObject.GetComponent<NavMeshAgent>();
        m_navMeshAgent.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_reachedTarget && Vector3.Distance(m_navMeshAgent.gameObject.transform.position, m_target.position) <= m_navMeshAgent.stoppingDistance + m_offestY)
        {
            m_navMeshAgent.transform.forward = m_target.forward;
            m_reachedTarget = true;
            m_navMeshAgent.enabled = false;

            NotificationManager.instance.NotifyArrived(m_targetStr);
        }
    }

    void SetDestination(Vector3 a_destination)
    {
        m_navMeshAgent.SetDestination(a_destination);
    }

    public void GoToTarget(Transform a_target, string a_siteEnumStr)
    {
        Debug.Log("Go to target");
        m_navMeshAgent.enabled = true;

        m_target = a_target;
        SetDestination(a_target.position);

        m_targetStr = a_siteEnumStr;

        m_reachedTarget = false;
    }
}
