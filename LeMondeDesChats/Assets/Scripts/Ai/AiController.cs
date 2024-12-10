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

    [SerializeField]
    private List<Transform> schoolWaypoints = new List<Transform>();

    [SerializeField]
    private List<Transform> wandererWaypoints = new List<Transform>();

    /// <summary>
    /// Enumeration of possible AI states
    /// </summary>
    public enum AiState
    {
        Work,
        Food,
        Rest,
        Wanderer,
        School
    }
    [SerializeField]
    public AiState currentState;

    [SerializeField]
    private float timeduration = 10f;
    [SerializeField]
    private float variationEtat = 2f;
    private float elapseTime;

    public float tiredness;
    private const float tirednessMax = 100f;
    private const float tirednessPerSec = 0.8f;

    public int age = 0;
    private int oldAge;

    /// <summary>
    /// Enumeration of resource types produced by the agent
    /// </summary>
    private enum ResourceType
    {
        Wood,
        Stone,
        Food
    }

    [SerializeField]
    private ResourceType resourceType;

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

    /// <summary>
    /// Initializes the AI controller by setting up waypoints and initial state
    /// </summary>
    public void Initialize()
    {
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
                Debug.LogWarning("Unknown TagOfWork. Defaulting to Wood.");
                break;
        }

        var workWaypointObjects = GameObject.FindGameObjectsWithTag(TagOfWork.stringTag);
        var foodWaypointObjects = GameObject.FindGameObjectsWithTag("FoodWaypoint");
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
        else if (TagOfWork.stringTag == "SchoolWaypoint")
        {
            currentState = AiState.School;
        }
        else
        {
            currentState = AiState.Work;
        }

        elapseTime = Random.Range(0f, timeduration + Random.Range(-variationEtat, variationEtat));
        tiredness = 0f;
        resourceCollected = false;
        DefineDestination();

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

    /// <summary>
    /// Handles the time pause event to stop or resume the agent
    /// </summary>
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

        if (currentState == AiState.Work || currentState == AiState.Food || this.tag == "Builder")
        {
            tiredness += tirednessPerSec * Time.deltaTime;
            tiredness = Mathf.Min(tiredness, tirednessMax);
        }

        if (tiredness >= tirednessMax && currentState != AiState.Food && oldAge != age)
        {
            currentState = AiState.Rest;
            RestCoord();
            elapseTime = 0f;
            resourceCollected = false;
            DefineDestination();
            oldAge = age;
            return;
        }

        if (currentState == AiState.Food && agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            if (!resourceCollected)
            {
                RessourcesGlobales.Instance.AjouterNourriture(-1);
                tiredness = 0f;
                currentState = AiState.Work;
                resourceCollected = true;
                DefineDestination();
            }
        }

        if (elapseTime >= timeduration + Random.Range(-variationEtat, variationEtat))
        {
            elapseTime = 0f;
            currentState = ObtenirEtatSuivant(currentState);
            resourceCollected = false;
            DefineDestination();
        }

        if (currentState == AiState.Work && IsAtWorkDestination())
        {
            if (!resourceCollected)
            {
                switch (resourceType)
                {
                    case ResourceType.Wood:
                        RessourcesGlobales.Instance.AjouterBois(1);
                        break;
                    case ResourceType.Stone:
                        RessourcesGlobales.Instance.AjouterPierre(1);
                        break;
                    case ResourceType.Food:
                        RessourcesGlobales.Instance.AjouterNourriture(1);
                        break;
                }
                resourceCollected = true;
            }
        }

        if (this.gameObject.tag == "Wanderer" && agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            DefineDestination();
        }

        if (currentState == AiState.School && agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            DefineDestination();
        }
    }

    /// <summary>
    /// Sets the agent to build at the specified position
    /// </summary>
    public void GoBuild(Transform Position)
    {
        currentState = AiState.Work;
        priorityDestination = Position;
        agent.SetDestination(Position.position);
    }

    /// <summary>
    /// Ends the building process and switches to Wanderer state
    /// </summary>
    public void EndBuild()
    {
        priorityDestination = null;
        currentState = AiState.Wanderer;
    }

    /// <summary>
    /// Checks if the agent has reached the work destination
    /// </summary>
    public bool IsAtWorkDestination()
    {
        return agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending;
    }

    /// <summary>
    /// Defines and sets the next destination based on the current state
    /// </summary>
    void DefineDestination()
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
            if (currentState == AiState.Rest)
            {
                RessourcesGlobales.Instance.AddProsperity(-2);
                waypoints = wandererWaypoints;
            }
            return;
        }

        if (currentState == AiState.Wanderer)
        {
            int index = Random.Range(0, waypoints.Count);
            agent.SetDestination(waypoints[index].position + randomOffset);
        }
        else
        {
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
                Debug.LogWarning("Cannot find a waypoint for state " + currentState);
                agent.SetDestination(transform.position);
            }
        }
    }

    /// <summary>
    /// Ages the individual by one day and handles death if necessary
    /// </summary>
    public void AgeOneDay()
    {
        age++;
        if (age == 15)
        {
            Destroy(gameObject);
            RessourcesGlobales.Instance.RegisterVillagerAlive(-1);
            if (this.gameObject.tag == "Builder")
            {
                RessourcesGlobales.Instance.RegisterBuilderAlive(-1);
            }
        }
    }

    /// <summary>
    /// Called when the object is destroyed
    /// </summary>
    void OnDestroy()
    {
        restHouse.remainingRoom.Remove(this);
        TimeManager timeManager = FindObjectOfType<TimeManager>();

        if (timeManager != null)
        {
            timeManager.UnregisterIndividual(this);
        }
    }

    /// <summary>
    /// Determines the next state based on the current state
    /// </summary>
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
                return AiState.Wanderer;
            default:
                return AiState.Work;
        }
    }

    /// <summary>
    /// Sets the AI state to School and populates school waypoints
    /// </summary>
    public void SchoolState()
    {
        currentState = AiState.School;
        var schoolWaypointsObjects = GameObject.FindGameObjectsWithTag("SchoolWaypoint");

        foreach (var obj in schoolWaypointsObjects)
        {
            schoolWaypoints.Add(obj.transform);
        }
    }

    /// <summary>
    /// Changes the AI state to Food
    /// </summary>
    public void RestState()
    {
        currentState = AiState.Food;
    }

    /// <summary>
    /// Coordinates the AI to rest by assigning a rest waypoint
    /// </summary>
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

    /// <summary>
    /// Draws gizmos in the Unity Editor to visualize the agent's path
    /// </summary>
    public void OnDrawGizmos()
    {
        if (agent != null && agent.path != null && agent.path.corners.Length > 0 && agent.path.corners.Length % 2 != 1)
        {
            var path = agent.path;
            Gizmos.DrawLineList(path.corners);
        }
    }
}
