using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AiController : MonoBehaviour
{
    [Header("Waypoints Settings")]
    [SerializeField]
    private GameObject waypointsManager;

    private NavMeshAgent agent;

    [Header("Layer Settings")]
    [SerializeField]
    private LayerMask ressourceLayer;

    [SerializeField]
    private Tag TagOfWork;

    private List<Transform> workWaypoints = new List<Transform>();
    private List<Transform> foodWaypoints = new List<Transform>();
    private List<Transform> restWaypoints = new List<Transform>();

    private enum AiState
    {
        Travail,
        Nourriture,
        Repos
    }

    private AiState etatActuel;
    [SerializeField]
    private float dureeEtat = 10f; // Durée de chaque état en secondes
    private float tempsEcoule;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        var workWaypointObjects = GameObject.FindGameObjectsWithTag(TagOfWork);
        var foodWaypointObjects = GameObject.FindGameObjectsWithTag("FoodWaypoint");
        var restWaypointObjects = GameObject.FindGameObjectsWithTag("RestWaypoint");

        foreach (var obj in workWaypointObjects)
        {
            workWaypoints.Add(obj.transform);
        }

        foreach (var obj in foodWaypointObjects)
        {
            foodWaypoints.Add(obj.transform);
        }

        foreach (var obj in restWaypointObjects)
        {
            restWaypoints.Add(obj.transform);
        }

        etatActuel = AiState.Travail;
        tempsEcoule = Random.Range(0f, dureeEtat);
        DefinirDestination();
    }

    void Update()
    {
        tempsEcoule += Time.deltaTime;
        if (tempsEcoule >= dureeEtat)
        {
            tempsEcoule = 0f;
            etatActuel = ObtenirEtatSuivant(etatActuel);
            Debug.Log("Changement d'état: " + etatActuel);
            DefinirDestination();
        }

        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            Debug.Log("Arrivé à destination, définir une nouvelle destination");
            DefinirDestination(); // L'agent est arrivé à destination, définir une nouvelle destination
        }
    }

    AiState ObtenirEtatSuivant(AiState etatActuel)
    {
        switch (etatActuel)
        {
            case AiState.Travail:
                return AiState.Nourriture;
            case AiState.Nourriture:
                return AiState.Repos;
            case AiState.Repos:
                return AiState.Travail;
            default:
                return AiState.Travail;
        }
    }

    void DefinirDestination()
    {
        switch (etatActuel)
        {
            case AiState.Travail:
                if (workWaypoints.Count > 0)
                {
                    int index = Random.Range(0, workWaypoints.Count);
                    Debug.Log("Nouvelle destination de travail: " + workWaypoints[index].position);
                    agent.SetDestination(workWaypoints[index].position);
                }
                break;
            case AiState.Nourriture:
                if (foodWaypoints.Count > 0)
                {
                    int index = Random.Range(0, foodWaypoints.Count);
                    Debug.Log("Nouvelle destination de nourriture: " + foodWaypoints[index].position);
                    agent.SetDestination(foodWaypoints[index].position);
                }
                break;
            case AiState.Repos:
                if (restWaypoints.Count > 0)
                {
                    int index = Random.Range(0, restWaypoints.Count);
                    Debug.Log("Nouvelle destination de repos: " + restWaypoints[index].position);
                    agent.SetDestination(restWaypoints[index].position);
                }
                break;
        }
    }
}
