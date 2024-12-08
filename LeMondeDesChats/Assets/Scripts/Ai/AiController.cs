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

    [SerializeField]
    private List<Transform> workWaypoints = new List<Transform>();
    [SerializeField]
    private List<Transform> foodWaypoints = new List<Transform>();
    [SerializeField]
    private List<Transform> restWaypoints = new List<Transform>();

    // #
    [SerializeField]
    private List<Transform> wandererWaypoints = new List<Transform>();

    private enum AiState
    {
        Travail,
        Nourriture,
        Repos,
        Wanderer // #
    }

    private AiState etatActuel;

    [SerializeField]
    private float dureeEtat = 10f; // Durée de chaque état en secondes
    [SerializeField]
    private float variationEtat = 2f; // Variation aléatoire de la durée de l'état
    private float tempsEcoule;

    public float fatigue;
    private const float fatigueMax = 100f;          // Seuil maximal de fatigue
    private const float fatigueParSeconde = 0.8f;   // Fatigue accumulée par seconde

    public int age = 0; // Âge de l'individu en jours

    // Type de ressource produit par l'agent
    private enum ResourceType
    {
        Bois,
        Pierre,
        Nourriture
    }

    [SerializeField]
    private ResourceType resourceType;

    // Nouveau : Flag pour éviter la production multiple de ressources
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
        // Déterminer le type de ressource en fonction du tag
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
            default:
                resourceType = ResourceType.Bois;
                Debug.LogWarning("TagOfWork inconnu. Défaut à Bois.");
                break;
        }

        // Trouver les waypoints en fonction des tags
        var workWaypointObjects = GameObject.FindGameObjectsWithTag(TagOfWork.stringTag);
        var foodWaypointObjects = GameObject.FindGameObjectsWithTag("FoodWaypoint");
        var restWaypointObjects = GameObject.FindGameObjectsWithTag("RestWaypoint");

        // #
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

        // #
        foreach (var obj in wandererWaypointsObjects)
        {
            wandererWaypoints.Add(obj.transform);
        }

        //#
        etatActuel = TagOfWork.stringTag == "WandererWaypoint" ? AiState.Wanderer : AiState.Travail;


        tempsEcoule = Random.Range(0f, dureeEtat + Random.Range(-variationEtat, variationEtat));
        fatigue = 0f; // Initialiser la fatigue à 0
        resourceCollected = false; // Initialiser le flag
        DefinirDestination();

        // Enregistrer cet individu auprès du TimeManager
        TimeManager timeManager = FindObjectOfType<TimeManager>();
        if (timeManager != null)
        {
            timeManager.RegisterIndividual(this);
        }
    }

    void Update()
    {
        tempsEcoule += Time.deltaTime;

        // Augmenter la fatigue lorsque l'agent travaille ou cherche de la nourriture
        if (etatActuel == AiState.Travail || etatActuel == AiState.Nourriture)
        {
            fatigue += fatigueParSeconde * Time.deltaTime;
            fatigue = Mathf.Min(fatigue, fatigueMax); // Limiter la fatigue au maximum
        }

        if (etatActuel == AiState.Wanderer) // #
        {

        }

        // Vérifier si l'agent est fatigué
        if (fatigue >= fatigueMax && etatActuel != AiState.Nourriture)
        {
            etatActuel = AiState.Nourriture;
            tempsEcoule = 0f; // Réinitialiser le temps écoulé pour le nouvel état
            resourceCollected = false; // Réinitialiser le flag
            DefinirDestination();
            return;
        }

        if (tempsEcoule >= dureeEtat + Random.Range(-variationEtat, variationEtat))
        {
            tempsEcoule = 0f;
            etatActuel = ObtenirEtatSuivant(etatActuel);
            resourceCollected = false; // Réinitialiser le flag lors du changement d'état
            DefinirDestination();
        }

        // Ajouter des ressources lors de l'accomplissement de l'état Travail
        if (etatActuel == AiState.Travail && agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
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
                resourceCollected = true; // Marquer comme collecté
            }
        }

        // Consommer de la nourriture si en état Nourriture et arrivé à destination
        if (etatActuel == AiState.Nourriture && agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            if (!resourceCollected)
            {
                RessourcesGlobales.Instance.AjouterNourriture(-1); // Consomme 1 nourriture
                fatigue = 0f; // Réinitialiser la fatigue après avoir mangé
                etatActuel = AiState.Travail; // Revenir au travail
                resourceCollected = true; // Marquer comme consommé
                DefinirDestination();
            }
        }
    }

    AiState ObtenirEtatSuivant(AiState etatActuel)
    {
        switch (etatActuel)
        {
            case AiState.Travail:
                return AiState.Travail; // Continue à travailler
            case AiState.Nourriture:
                return AiState.Travail; // Retour au travail après avoir mangé
            case AiState.Wanderer:
                return AiState.Wanderer;
            case AiState.Repos:
                fatigue = 0f;
                return AiState.Travail;
            default:
                return AiState.Travail;
        }
    }

    void DefinirDestination()
    {
        Vector3 randomOffset = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
        Transform closestWaypoint = null;
        float closestDistance = Mathf.Infinity;

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

            case AiState.Wanderer: // #
                if (wandererWaypoints.Count > 0)
                {
                    int index = Random.Range(0, wandererWaypoints.Count);
                    randomOffset = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
                    agent.SetDestination(wandererWaypoints[index].position + randomOffset);
                }
                break;

        }

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
        }
    }

    public void AgeOneDay()
    {
        age++;
        Debug.Log(gameObject.name + " a maintenant " + age + " jours.");
        // Ajouter de la logique supplémentaire en fonction de l'âge (par exemple, mourir après un certain âge)
    }

    void OnDestroy()
    {
        // Se désenregistrer du TimeManager lorsque l'agent est détruit
        TimeManager timeManager = FindObjectOfType<TimeManager>();
        if (timeManager != null)
        {
            timeManager.UnregisterIndividual(this);
        }
    }
}
