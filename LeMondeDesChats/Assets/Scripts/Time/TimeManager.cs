using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("Time Settings")]
    public float dayDuration = 60f; // Dur�e totale d'un cycle jour/nuit en secondes
    private float dayTimer = 0f;
    public int currentDay = 0;

    public float timeScale; // #

    [Header("Population Settings")]
    public float spawnInterval = 30f; // Intervalle pour l'ajout de nouveaux individus
    private float spawnTimer = 0f;
    public GameObject individualPrefab; // Prefab de l'agent AI
    private List<AiController> individuals = new List<AiController>();

    [SerializeField]
    private CanvasManager canvasManager;

    [SerializeField]
    private Light directionalLight;

    private bool isTimePaused = false;

    public delegate void TimePauseHandler(bool isPaused);
    public static event TimePauseHandler OnTimePause;

    private void Start()
    {
        canvasManager.timeSlider.maxValue = dayDuration;
        Time.timeScale = timeScale;
    }

    void Update()
    {
        if (!isTimePaused)
        {
            // Gestion du temps de la journ�e
            dayTimer += Time.deltaTime;
            canvasManager.updateTimeSlider(dayTimer);

            // Calcul de la fraction du cycle (de 0 � 1)
            float cycleFraction = dayTimer / dayDuration;

            // Calcul de l'angle du soleil (de 0� � 360�)
            float sunAngle = cycleFraction * 360f;

            // Mise � jour de la rotation de la lumi�re directionnelle
            directionalLight.transform.rotation = Quaternion.Euler(new Vector3(sunAngle - 90f, 0f, 0f));

            // Ajustement de l'intensit� de la lumi�re pour simuler le jour et la nuit
            float lightIntensity = Mathf.Clamp01(Mathf.Sin(cycleFraction * Mathf.PI));
            directionalLight.intensity = lightIntensity;

            // Optionnel : Ajustement de la couleur de la lumi�re pour simuler les levers et couchers de soleil
            UpdateLightColor(cycleFraction);

            if (dayTimer >= dayDuration)
            {
                dayTimer = 0f;
                currentDay++;
                EndOfDay();
            }

            // Gestion du temps pour l'ajout de nouveaux individus
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval)
            {
                spawnTimer = 0f;
                SpawnNewIndividual();
            }
        }
    }

    void UpdateLightColor(float cycleFraction)
    {
        Color sunriseColor = new Color(1f, 0.5f, 0f); // Orange du lever de soleil
        Color noonColor = Color.white;                // Lumi�re blanche du midi
        Color sunsetColor = new Color(1f, 0.5f, 0f);  // Orange du coucher de soleil
        Color nightColor = Color.black;               // Noir pour la nuit

        if (cycleFraction <= 0.25f) // Lever du soleil
        {
            float t = cycleFraction / 0.25f;
            directionalLight.color = Color.Lerp(nightColor, sunriseColor, t);
        }
        else if (cycleFraction <= 0.5f) // Journ�e
        {
            float t = (cycleFraction - 0.25f) / 0.25f;
            directionalLight.color = Color.Lerp(sunriseColor, noonColor, t);
        }
        else if (cycleFraction <= 0.75f) // Apr�s-midi vers coucher du soleil
        {
            float t = (cycleFraction - 0.5f) / 0.25f;
            directionalLight.color = Color.Lerp(noonColor, sunsetColor, t);
        }
        else // Nuit
        {
            float t = (cycleFraction - 0.75f) / 0.25f;
            directionalLight.color = Color.Lerp(sunsetColor, nightColor, t);
        }
    }

    void EndOfDay()
    {
        // Faire vieillir tous les individus
        foreach (AiController ai in individuals)
        {
            ai.AgeOneDay();
        }

        // Consommation de nourriture
        int totalIndividus = individuals.Count;
        int nourritureDisponible = RessourcesGlobales.Instance.nourriture;

        if (nourritureDisponible >= totalIndividus)
        {
            RessourcesGlobales.Instance.nourriture -= totalIndividus;
        }
        else
        {
            // Pas assez de nourriture, des individus meurent al�atoirement
            int deficit = totalIndividus - nourritureDisponible;
            RessourcesGlobales.Instance.nourriture = 0;

            for (int i = 0; i < deficit; i++)
            {
                if (individuals.Count > 0)
                {
                    int index = Random.Range(0, individuals.Count);
                    AiController individuAffame = individuals[index];
                    individuals.RemoveAt(index);
                    Destroy(individuAffame.gameObject);
                    Debug.Log(individuAffame.gameObject.name + " est mort de faim.");
                }
            }
        }
        Debug.Log("Fin du jour " + currentDay + ". Tous les individus ont vieilli.");
    }

    void SpawnNewIndividual()
    {
        // D�terminer la position de spawn (� adapter selon vos besoins)
        Vector3 spawnPosition = GetRandomSpawnPosition();
        Quaternion spawnRotation = Quaternion.identity;

        GameObject newIndividual = Instantiate(individualPrefab, spawnPosition, spawnRotation);
        RessourcesGlobales.Instance.RegisterVillagerAlive(1);
        if (newIndividual.tag == "Builder")
        {
            RessourcesGlobales.Instance.RegisterBuilderAlive(1);
        }
            

        // Ajouter le nouvel individu � la liste
        AiController aiController = newIndividual.GetComponent<AiController>();
        if (aiController != null)
        {
            individuals.Add(aiController);
            aiController.Initialize(); // Initialiser l'agent si n�cessaire
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        // Impl�mentez votre logique pour d�terminer une position de spawn appropri�e
        // Par exemple, une position al�atoire dans une zone d�finie
        return new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
    }

    public void RegisterIndividual(AiController ai)
    {
        if (!individuals.Contains(ai))
            individuals.Add(ai);
    }

    public void UnregisterIndividual(AiController ai)
    {
        if (individuals.Contains(ai))
            individuals.Remove(ai);
    }


    public void PauseTime()
    {
        if (!isTimePaused)
        {
            isTimePaused = true;
            Time.timeScale = 0f;

            // Notifier les agents AI
            OnTimePause?.Invoke(true);
        }
        else
        {
            isTimePaused = false;
            Time.timeScale = 1f;

            // Notifier les agents AI
            OnTimePause?.Invoke(false);
            QueueActions.StartActions();
            QueueActions.ClearActions();

        }

    }

   /* public void ResumeTime()
    {
        isTimePaused = false;
        Time.timeScale = 1f;

        // Notifier les agents AI
        OnTimePause?.Invoke(false);
    }*/
}
