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
        Work,
        Food,
        Rest,
        Wanderer,
        School// #
    }
    [SerializeField]
    public AiState currentState;

    [SerializeField]
    private float timeduration = 10f; // Durée de chaque état en secondes
    [SerializeField]
    private float variationEtat = 2f; // Variation aléatoire de la durée de l'état
    private float elapseTime;

    public float tiredness;
    private const float tirednessMax = 100f;          // Seuil maximal de tiredness
    private const float tirednessPerSec = 0.8f;   // Fatigue accumulée par seconde

    public int age = 0; // Âge de l'individu en jours
    private int oldAge;
    // Type de ressource produit par l'agent
    private enum ResourceType
    {
        Wood,
        Stone,
        Food
    }

    [SerializeField]
    private ResourceType resourceType;

    // Nouveau : Flag pour éviter la production multiple de ressources
    private bool resourceCollected = false;

    private Transform priorityDestination;
    private RestHouse restHouse;

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
                resourceType = ResourceType.Wood;
                break;
            case "MineWaypoint":
                resourceType = ResourceType.Stone;
                break;
            case "BushWaypoint":
                resourceType = ResourceType.Food;
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
                resourceType = ResourceType.Wood;
                Debug.LogWarning("TagOfWork inconnu. Défaut à Bois.");
                break;
        }

        // Trouver les waypoints en fonction des tags
        var workWaypointObjects = GameObject.FindGameObjectsWithTag(TagOfWork.stringTag);
        var foodWaypointObjects = GameObject.FindGameObjectsWithTag("FoodWaypoint");

 

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

        foreach (var obj in wandererWaypointsObjects)
        {
            wandererWaypoints.Add(obj.transform);
        }


        if (TagOfWork.stringTag == "RestWaypoint" || this.gameObject.tag == "Wanderer")
        {

            currentState = AiState.Wanderer;
        }
        else if(TagOfWork.stringTag == "SchoolWaypoint")
        {
            currentState = AiState.School;
        }
        else
        {
            currentState = AiState.Work;
        }



        elapseTime = Random.Range(0f, timeduration + Random.Range(-variationEtat, variationEtat));
        tiredness = 0f; // Initialiser la tiredness à 0
        resourceCollected = false; // Initialiser le flag
        DefinirDestination();

        // Enregistrer cet individu auprès du TimeManager
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

        elapseTime += Time.deltaTime;


        // Augmenter la tiredness lorsque l'agent travaille ou cherche de la nourriture
        if (currentState == AiState.Work || currentState == AiState.Food || this.tag == "Builder")
        {
            tiredness += tirednessPerSec * Time.deltaTime;
            tiredness = Mathf.Min(tiredness, tirednessMax); // Limiter la tiredness au maximum
        }


        // Vérifier si l'agent est fatigué
        if (tiredness >= tirednessMax && currentState != AiState.Food && oldAge != age)
        {
            currentState = AiState.Rest;
            RestCoord();
            elapseTime = 0f; 
            resourceCollected = false; 
            DefinirDestination();
            oldAge = age;
            return;
        }

        // Consommer de la nourriture si en état Food et arrivé à destination
        if (currentState == AiState.Food && agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            if (!resourceCollected)
            {
                RessourcesGlobales.Instance.AjouterNourriture(-1); // Consomme 1 nourriture
                tiredness = 0f; // Réinitialiser la tiredness après avoir mangé
                currentState = AiState.Work; // Revenir au travail
                resourceCollected = true; // Marquer comme consommé
                DefinirDestination();
            }
        }


        if (elapseTime >= timeduration + Random.Range(-variationEtat, variationEtat))
        {
            elapseTime = 0f;
            currentState = ObtenirEtatSuivant(currentState);
            resourceCollected = false; // Réinitialiser le flag lors du changement d'état
            DefinirDestination();
        }

        // Ajouter des ressources lors de l'accomplissement de l'état Work
        if (currentState == AiState.Work && IsAtWorkDestination())
        {
            if (!resourceCollected)
            {
                switch (resourceType)
                {
                    case ResourceType.Wood:
                        RessourcesGlobales.Instance.AjouterBois(1); // Ajoute 1 bois
                        break;
                    case ResourceType.Stone:
                        RessourcesGlobales.Instance.AjouterPierre(1); // Ajoute 1 pierre
                        break;
                    case ResourceType.Food:
                        RessourcesGlobales.Instance.AjouterNourriture(1); // Ajoute 1 nourriture
                        break;
                }
                resourceCollected = true; // Marquer comme collecté
            }
        }

        // Gérer le mouvement du Wanderer lorsqu'il atteint sa destination
        if (this.gameObject.tag == "Wanderer" && agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            DefinirDestination();
        }

        if (currentState == AiState.School && agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {

            DefinirDestination();
        }

    }

    public void GoBuild(Transform Position)
    {
        currentState = AiState.Work;
        priorityDestination = Position;
        agent.SetDestination(Position.position);
        //Debug.Log(Position.position);
    }

    public void EndBuild()
    {
        priorityDestination = null;
        currentState = AiState.Wanderer;
    }

    public bool IsAtWorkDestination()
    {
        return agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending;
    }

    void DefinirDestination()
    {
        if (priorityDestination != null)
            return;
        Vector3 randomOffset = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
        List<Transform> waypoints = new List<Transform>();

        switch (currentState)
        {
            case AiState.Work:
                waypoints = workWaypoints;
                break;
            case AiState.Food:
                waypoints = foodWaypoints;
                break;
            case AiState.Rest:
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
            if(currentState == AiState.Rest)
            {
                RessourcesGlobales.Instance.AddProsperity(-2);
                waypoints = wandererWaypoints;
            }
            return;
        }

        if (currentState == AiState.Wanderer)
        {
            // Choisir un waypoint aléatoire pour le Wanderer
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

            }
            else
            {
                Debug.LogWarning("Impossible de trouver un waypoint pour l'état " + currentState);
                agent.SetDestination(transform.position); // Ne pas bouger
            }
        }
    }
    public void AgeOneDay()
    {
        age++;
        if (age == 5)
        {
            Destroy(gameObject);
            RessourcesGlobales.Instance.RegisterVillagerAlive(-1);
            if(this.gameObject.tag == "Builder")
            {
                RessourcesGlobales.Instance.RegisterBuilderAlive(-1);
            }
                
        }

        //Debug.Log(gameObject.name + " a maintenant " + age + " jours.");
        // Ajouter de la logique supplémentaire en fonction de l'âge (par exemple, mourir après un certain âge)
    }

    void OnDestroy()
    {
        restHouse.remainingRoom.Remove(this);

        // Se désenregistrer du TimeManager lorsque l'agent est détruit
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
            case AiState.Work:
                return AiState.Work;
            case AiState.Rest:
                return AiState.Food;
            case AiState.Food:
                return AiState.Work;
            case AiState.School:
                return AiState.School;
            case AiState.Wanderer:
                return AiState.Wanderer; // Wanderer reste dans le même état
            default:
                return AiState.Work;
        }
    }

    public void SchoolState()
    {
        currentState = AiState.School;
        var schoolWaypointsObjects = GameObject.FindGameObjectsWithTag("SchoolWaypoint");

        foreach (var obj in schoolWaypointsObjects)
        {
            schoolWaypoints.Add(obj.transform);
        }
    }

    public void RestState()
    {
        currentState = AiState.Food;
    }

    public void RestCoord()
    {
        var restWaypointsObjects = GameObject.FindGameObjectsWithTag("RestWaypoint");

        foreach (var obj in restWaypointsObjects)
        {
            restHouse = obj.GetComponent<RestHouse>();
            if (restHouse.remainingRoom.Count<5)
            {
                restHouse.remainingRoom.Add(this);
                restWaypoints.Add(obj.transform);
                break;
            }
            
        }
    }

    public void OnDrawGizmos()
    {

        if (agent != null && agent.path != null && agent.path.corners.Length > 0 && agent.path.corners.Length%2 != 1)
        {
            var path = agent.path;
            Gizmos.DrawLineList(path.corners);
        }
    }
}
