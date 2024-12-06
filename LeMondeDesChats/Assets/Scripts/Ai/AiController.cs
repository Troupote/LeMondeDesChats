using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AiController : MonoBehaviour
{
    [Header("Waypoints Settings")]
    [SerializeField]
    private GameObject waypointsManager;

    private int currentWaypointIndex = 0;

    private Transform target;

    private NavMeshAgent agent;

    [Space]
    [Header("Layer Settings")]
    [SerializeField]
    private LayerMask ressourceLayer;
    private List<Transform> waypoints; 

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        waypoints = waypointsManager.GetComponentsInChildren<Transform>().ToList();
        Debug.Log("Waypoints List:");
        foreach (var waypoint in waypoints)
        {
            Debug.Log(waypoint.name);
        }
        Debug.Log("Total Waypoints: " + waypoints.Count);
    }

    void Update()
    {
        if (target != null)
        {
            agent.SetDestination(target.position);
        }

        

        if ((agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending) & waypoints.Count != 0)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

}