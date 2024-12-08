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

    private enum AiState
    {
        Travail,
        Nourriture,
        Repos
    }

    private AiState etatActuel;

    [SerializeField]
    private float dureeEtat = 10f; // Dur�e de chaque �tat en secondes
    [SerializeField]
    private float variationEtat = 2f; // Variation al�atoire de la dur�e de l'�tat
    private float tempsEcoule;

    public float fatigue;
    private const float fatigueMax = 100f;          // Seuil maximal de fatigue
    private const float fatigueParSeconde = 0.8f;   // Fatigue accumul�e par seconde

    public int age = 0; // �ge de l'individu en jours

    // Nouveau : Type de ressource produit par l'agent
    private enum ResourceType
    {
        Bois,
        Pierre,
        Nourriture
    }

    [SerializeField]
    private ResourceType resourceType;

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
            default:
                resourceType = ResourceType.Bois;
                Debug.LogWarning("TagOfWork inconnu. D�faut � Bois.");
                break;
        }

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
        tempsEcoule = Random.Range(0f, dureeEtat + Random.Range(-variationEtat, variationEtat));
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
        if (fatigue >= fatigueMax && etatActuel != AiState.Nourriture)
        {
            etatActuel = AiState.Nourriture;
            tempsEcoule = 0f; // R�initialiser le temps �coul� pour le nouvel �tat
            DefinirDestination();
            return;
        }

        if (tempsEcoule >= dureeEtat + Random.Range(-variationEtat, variationEtat))
        {
            tempsEcoule = 0f;
            etatActuel = ObtenirEtatSuivant(etatActuel);
            DefinirDestination();
        }

        // Ajouter des ressources lors de l'accomplissement des �tats de Travail
        if (etatActuel == AiState.Travail && agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
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
        }

        // Consommer de la nourriture si en �tat Nourriture et arriv� � destination
        if (etatActuel == AiState.Nourriture && agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            RessourcesGlobales.Instance.AjouterNourriture(-1); // Consomme 1 nourriture
            fatigue = 0f; // R�initialiser la fatigue apr�s avoir mang�
            etatActuel = AiState.Travail; // Revenir au travail
            DefinirDestination();
        }
    }

    AiState ObtenirEtatSuivant(AiState etatActuel)
    {
        switch (etatActuel)
        {
            case AiState.Travail:
                return AiState.Travail; // Continue � travailler
            case AiState.Nourriture:
                return AiState.Travail; // Retour au travail apr�s avoir mang�
            case AiState.Repos:
                fatigue = 0f;
                return AiState.Travail;
            default:
                return AiState.Travail;
        }
    }

    void DefinirDestination()
    {
        Vector3 randomOffset = Vector3.zero;
        switch (etatActuel)
        {
            case AiState.Travail:
                if (workWaypoints.Count > 0)
                {
                    int index = Random.Range(0, workWaypoints.Count);
                    randomOffset = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
                    agent.SetDestination(workWaypoints[index].position + randomOffset);
                }
                break;

            case AiState.Nourriture:
                if (foodWaypoints.Count > 0)
                {
                    int index = Random.Range(0, foodWaypoints.Count);
                    randomOffset = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
                    agent.SetDestination(foodWaypoints[index].position + randomOffset);
                }
                break;

            case AiState.Repos:
                if (restWaypoints.Count > 0)
                {
                    int index = Random.Range(0, restWaypoints.Count);
                    randomOffset = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
                    agent.SetDestination(restWaypoints[index].position + randomOffset);
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
