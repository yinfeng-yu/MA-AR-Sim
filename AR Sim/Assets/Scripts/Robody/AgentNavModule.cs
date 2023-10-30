using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AgentNavModule : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;

    public SiteEnum destSiteEnum;
    public Vector3 destLocation;

    // public Site destinationSite;
    // public LocationsList locationsList;

    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        destLocation = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // if (navMeshAgent.destination != destLocation)
        // {
        //     navMeshAgent.destination = destLocation;
        // }

        navMeshAgent.destination = target.position;
    }


    // public void SetDestination(SiteEnum a_destSiteEnum)
    // {
    //     destSiteEnum = a_destSiteEnum;
    //     Vector3? foundDestLocation = SitesManager.instance.GetLocation(a_destSiteEnum);
    //     if (foundDestLocation != null) destLocation = (Vector3) foundDestLocation;
    // }

    // public void SetDestination(Site _destSite)
    // {
    //     foreach (var sl in locationsList.sitesLocations)
    //     {
    //         if (sl.site == _destSite)
    //         {
    //             navMeshAgent.destination = sl.location;
    //             return;
    //         }
    //     }
    // }
    // 
    // public void SetDestination(int patientId)
    // {
    //     destSite = locationsList.GetPatientSite(patientId);
    // }
}