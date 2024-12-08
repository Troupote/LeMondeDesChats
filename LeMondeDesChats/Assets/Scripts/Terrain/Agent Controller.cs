using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class AgentController : MonoBehaviour
{
    public Transform target;
    private NavMeshAgent agent;
    private GameObject agentGO;
    private Vector3 initialPose;
    public int currentWaypointIndex = 0;



    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agentGO = agent.gameObject;
        initialPose = transform.position;
    }


    public void SetNextWaypoint(Transform[] waypoints)
    {
        Vector3 targetPosition = target.position;
        if (!NavMesh.SamplePosition(agent.transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            Debug.LogWarning("L'agent n'est pas sur le NavMesh. Repositionnement...");
            agent.transform.position = waypoints[550].position;
            Debug.Log(agent.isOnNavMesh);
        }

        targetPosition = waypoints[200].position;
        NavMeshHit navMeshHit;

        if (!NavMesh.SamplePosition(targetPosition, out navMeshHit, 1.0f, NavMesh.AllAreas))
        {
            Debug.Log(true);
        }
        
        agent.SetDestination(targetPosition);
        //agent.SetDestination(waypoints[currentWaypointIndex].position);
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        initialPose = waypoints[currentWaypointIndex].position;
        Debug.Log(agent.pathPending);
    }
}