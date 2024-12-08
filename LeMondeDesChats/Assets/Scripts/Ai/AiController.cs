using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiController : MonoBehaviour
{
    [Header("Waypoints Settings")]
    [SerializeField]
    private Tag TagOfWork;

    private NavMeshAgent agent;

    [Header("Layer Settings")]
    [SerializeField]
    private LayerMask ressourceLayer;

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
    private float dureeEtat = 10f; // Dur�e de chaque �tat en secondes
    private float tempsEcoule;

    public float fatigue;
    private const float fatigueMax = 100f;         // Seuil maximal de fatigue
    private const float fatigueParSeconde = 5f;    // Fatigue accumul�e par seconde

    public int age = 0; // �ge de l'individu en jours

    // D�placer l'initialisation de 'agent' dans Awake()
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        // Trouver les waypoints en fonction des tags
        var workWaypointObjects = GameObject.FindGameObjectsWithTag(TagOfWork.stringTag);
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
        fatigue = 0f; // Initialiser la fatigue � 0
        DefinirDestination();

        // Enregistrer cet individu aupr�s du TimeManager
        TimeManager timeManager = FindObjectOfType<TimeManager>();
        if (timeManager != null)
        {
            timeManager.RegisterIndividual(this);
        }
    }

    void Update()
    {
        tempsEcoule += Time.deltaTime;

        // Augmenter la fatigue lorsque l'agent travaille ou se d�place
        if (etatActuel == AiState.Travail || etatActuel == AiState.Nourriture)
        {
            fatigue += fatigueParSeconde * Time.deltaTime;
            fatigue = Mathf.Min(fatigue, fatigueMax); // Limiter la fatigue au maximum
        }

        // V�rifier si l'agent est fatigu�
        if (fatigue >= fatigueMax && etatActuel != AiState.Repos)
        {
            etatActuel = AiState.Repos;
            tempsEcoule = 0f; // R�initialiser le temps �coul� pour le nouvel �tat
            DefinirDestination();
            return;
        }

        if (tempsEcoule >= dureeEtat)
        {
            tempsEcoule = 0f;
            etatActuel = ObtenirEtatSuivant(etatActuel);
            DefinirDestination();
        }
    }

    AiState ObtenirEtatSuivant(AiState etatActuel)
    {
        switch (etatActuel)
        {
            case AiState.Travail:
                return AiState.Nourriture;

            case AiState.Nourriture:
                if (fatigue >= fatigueMax)
                {
                    return AiState.Repos;
                }
                else
                {
                    return AiState.Travail;
                }

            case AiState.Repos:
                fatigue = 0f;
                return AiState.Nourriture;

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
                    agent.SetDestination(workWaypoints[index].position);
                }
                break;

            case AiState.Nourriture:
                if (foodWaypoints.Count > 0)
                {
                    int index = Random.Range(0, foodWaypoints.Count);
                    agent.SetDestination(foodWaypoints[index].position);
                }
                break;

            case AiState.Repos:
                if (restWaypoints.Count > 0)
                {
                    int index = Random.Range(0, restWaypoints.Count);
                    agent.SetDestination(restWaypoints[index].position);
                }
                break;
        }
    }

    public void AgeOneDay()
    {
        age++;
        Debug.Log(gameObject.name + " a maintenant " + age + " jours.");
        // Ajouter de la logique suppl�mentaire en fonction de l'�ge (par exemple, mourir apr�s un certain �ge)
    }

    void OnDestroy()
    {
        // Se d�senregistrer du TimeManager lorsque l'agent est d�truit
        TimeManager timeManager = FindObjectOfType<TimeManager>();
        if (timeManager != null)
        {
            timeManager.UnregisterIndividual(this);
        }
    }
}
