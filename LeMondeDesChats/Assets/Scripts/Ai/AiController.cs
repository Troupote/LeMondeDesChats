using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class AiController : MonoBehaviour
{
    [Header("Waypoints Settings")]
    [SerializeField]
    private Tag TagOfWork;

    private NavMeshAgent agent;

    [Header("Layer Settings")]
    [SerializeField]
    private LayerMask ressourceLayer;

    [SerializeField]
    private List<Transform> workWaypoints = new List<Transform>();
    [SerializeField]
    private List<Transform> foodWaypoints = new List<Transform>();
    [SerializeField]
    private List<Transform> restWaypoints = new List<Transform>();

    [SerializeField]
    private List<Transform> schoolWaypoints = new List<Transform>();

    // #
    [SerializeField]
    private List<Transform> wandererWaypoints = new List<Transform>();

    public enum AiState
    {
        Travail,
        Nourriture,
        Repos,
        Wanderer,
        School// #
    }
    [SerializeField]
    public AiState etatActuel;

    [SerializeField]
    private float dureeEtat = 10f; // Dur�e de chaque �tat en secondes
    [SerializeField]
    private float variationEtat = 2f; // Variation al�atoire de la dur�e de l'�tat
    private float tempsEcoule;

    public float fatigue;
    private const float fatigueMax = 100f;          // Seuil maximal de fatigue
    private const float fatigueParSeconde = 0.8f;   // Fatigue accumul�e par seconde

    public int age = 0; // �ge de l'individu en jours

    // Type de ressource produit par l'agent
    private enum ResourceType
    {
        Bois,
        Pierre,
        Nourriture
    }

    [SerializeField]
    private ResourceType resourceType;

    // Nouveau : Flag pour �viter la production multiple de ressources
    private bool resourceCollected = false;

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
        // D�terminer le type de ressource en fonction du tag
        switch (TagOfWork.stringTag)
        {
            case "ForestWaypoint":
                resourceType = ResourceType.Bois;
                break;
            case "MineWaypoint":
                resourceType = ResourceType.Pierre;
                break;
            case "BushWaypoint":
                resourceType = ResourceType.Nourriture;
                break;
            case "PlainWaypoint":
                break;
            case "RestWaypoint":
                break;
            case "WandererWaypoint":
                break;
            case "SchoolWaypoint":
                break;
            default:
                resourceType = ResourceType.Bois;
                Debug.LogWarning("TagOfWork inconnu. D�faut � Bois.");
                break;
        }

        // Trouver les waypoints en fonction des tags
        var workWaypointObjects = GameObject.FindGameObjectsWithTag(TagOfWork.stringTag);
        var foodWaypointObjects = GameObject.FindGameObjectsWithTag("FoodWaypoint");
        var restWaypointObjects = GameObject.FindGameObjectsWithTag("RestWaypoint");

        // #
        var schoolWaypointsObjects = GameObject.FindGameObjectsWithTag("SchoolWaypoint");
        var wandererWaypointsObjects = GameObject.FindGameObjectsWithTag("WandererWaypoint");

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

        foreach (var obj in schoolWaypointsObjects)
        {
            schoolWaypoints.Add(obj.transform);
        }

        // #

        wandererWaypoints = wandererWaypointsObjects[0].GetComponent<WandererScript>().targetWanderer;
        //#
        etatActuel = TagOfWork.stringTag == "WandererWaypoint" ? AiState.Wanderer : AiState.Travail;


        tempsEcoule = Random.Range(0f, dureeEtat + Random.Range(-variationEtat, variationEtat));
        fatigue = 0f; // Initialiser la fatigue � 0
        resourceCollected = false; // Initialiser le flag
        DefinirDestination();

        // Enregistrer cet individu aupr�s du TimeManager
        TimeManager timeManager = FindObjectOfType<TimeManager>();
        if (timeManager != null)
        {
            timeManager.RegisterIndividual(this);
        }
    }

    void OnEnable()
    {
        TimeManager.OnTimePause += HandleTimePause;
    }

    void OnDisable()
    {
        TimeManager.OnTimePause -= HandleTimePause;
    }

    private bool isPaused = false;
    private void HandleTimePause(bool paused)
    {
        isPaused = paused;
        if (agent != null)
        {
            agent.isStopped = isPaused;
        }
    }
    void Update()
    {
        if (isPaused)
            return;

        tempsEcoule += Time.deltaTime;

        // Augmenter la fatigue lorsque l'agent travaille ou cherche de la nourriture
        if (etatActuel == AiState.Travail || etatActuel == AiState.Nourriture)
        {
            fatigue += fatigueParSeconde * Time.deltaTime;
            fatigue = Mathf.Min(fatigue, fatigueMax); // Limiter la fatigue au maximum
        }

        // V�rifier si l'agent est fatigu�
        if (fatigue >= fatigueMax && etatActuel != AiState.Nourriture)
        {
            etatActuel = AiState.Nourriture;
            tempsEcoule = 0f; // R�initialiser le temps �coul� pour le nouvel �tat
            resourceCollected = false; // R�initialiser le flag
            DefinirDestination();
            return;
        }

        // Consommer de la nourriture si en �tat Nourriture et arriv� � destination
        if (etatActuel == AiState.Nourriture && agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            if (!resourceCollected)
            {
                RessourcesGlobales.Instance.AjouterNourriture(-1); // Consomme 1 nourriture
                fatigue = 0f; // R�initialiser la fatigue apr�s avoir mang�
                etatActuel = AiState.Travail; // Revenir au travail
                resourceCollected = true; // Marquer comme consomm�
                DefinirDestination();
            }
        }


        if (tempsEcoule >= dureeEtat + Random.Range(-variationEtat, variationEtat))
        {
            tempsEcoule = 0f;
            etatActuel = ObtenirEtatSuivant(etatActuel);
            resourceCollected = false; // R�initialiser le flag lors du changement d'�tat
            DefinirDestination();
        }

        // Ajouter des ressources lors de l'accomplissement de l'�tat Travail
        if (IsAtWorkDestination())
        {
            if (!resourceCollected)
            {
                switch (resourceType)
                {
                    case ResourceType.Bois:
                        RessourcesGlobales.Instance.AjouterBois(1); // Ajoute 1 bois
                        break;
                    case ResourceType.Pierre:
                        RessourcesGlobales.Instance.AjouterPierre(1); // Ajoute 1 pierre
                        break;
                    case ResourceType.Nourriture:
                        RessourcesGlobales.Instance.AjouterNourriture(1); // Ajoute 1 nourriture
                        break;
                }
                resourceCollected = true; // Marquer comme collect�
            }
        }

        // G�rer le mouvement du Wanderer lorsqu'il atteint sa destination
        if (etatActuel == AiState.Wanderer && agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            DefinirDestination();
        }

        if (etatActuel == AiState.School && agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {

            DefinirDestination();
        }

    }

    public void GoBuild(Transform Position)
    {
        etatActuel = AiState.Travail;
        agent.SetDestination(Position.position);
    }

    public bool IsAtWorkDestination()
    {
        return etatActuel == AiState.Travail && agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending;
    }

    void DefinirDestination()
    {
        Vector3 randomOffset = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
        List<Transform> waypoints = new List<Transform>();

        switch (etatActuel)
        {
            case AiState.Travail:
                waypoints = workWaypoints;
                break;
            case AiState.Nourriture:
                waypoints = foodWaypoints;
                break;
            case AiState.Repos:
                waypoints = restWaypoints;
                break;
            case AiState.Wanderer:
                waypoints = wandererWaypoints;
                break;
            case AiState.School:
                waypoints = schoolWaypoints;
                break;
        }
        if (waypoints.Count == 0)
        {
            Debug.LogWarning("Aucun waypoint trouv� pour l'�tat " + etatActuel);
            return;
        }


        if (etatActuel == AiState.Wanderer)
        {
            // Choisir un waypoint al�atoire pour le Wanderer
            int index = Random.Range(0, waypoints.Count);
            agent.SetDestination(waypoints[index].position + randomOffset);

        }
        else
        {
            // Trouver le waypoint le plus proche
            Transform closestWaypoint = null;
            float closestDistance = Mathf.Infinity;

            foreach (var waypoint in waypoints)
            {
                float distance = Vector3.Distance(transform.position, waypoint.position + randomOffset);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestWaypoint = waypoint;
                }
            }

            if (closestWaypoint != null)
            {
                agent.SetDestination(closestWaypoint.position + randomOffset);
                if (new Vector2((int)closestWaypoint.position.x/10, (int)closestWaypoint.position.z/10)== new Vector2((int)transform.position.x/10, (int)transform.position.y/10) && etatActuel == AiState.School)
                {
                    var obj = GameObject.FindGameObjectsWithTag("School");
                    obj[0].GetComponent<SchoolController>().AssignJob();
                }

            }
            else
            {
                Debug.LogWarning("Impossible de trouver un waypoint pour l'�tat " + etatActuel);
                agent.SetDestination(transform.position); // Ne pas bouger
            }
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
    private AiState ObtenirEtatSuivant(AiState etatActuel)
    {
        switch (etatActuel)
        {
            case AiState.Travail:
                return AiState.Travail;
            case AiState.Repos:
                return AiState.Nourriture;
            case AiState.Nourriture:
                return AiState.Travail;
            case AiState.School:
                return AiState.School;
            case AiState.Wanderer:
                return AiState.Wanderer; // Wanderer reste dans le m�me �tat
            default:
                return AiState.Travail;
        }
    }

    public void SchoolState()
    {
        etatActuel = AiState.School;
    }
}
