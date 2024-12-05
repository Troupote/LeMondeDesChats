using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AiController : MonoBehaviour
{
    [Header("Waypoints Settings")]
    [SerializeField]
    private List<Transform> waypoints;
    private int currentWaypointIndex = 0;

    private Transform target;

    private NavMeshAgent agent;

    [Space]
    [Header("Layer Settings")]
    [SerializeField]
    private LayerMask ressourceLayer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
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
