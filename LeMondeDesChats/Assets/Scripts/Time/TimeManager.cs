using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("Time Settings")]
    public float dayDuration = 60f; // Durée totale d'un cycle jour/nuit en secondes
    private float dayTimer = 0f;
    public int currentDay = 0;

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
    }

    void Update()
    {
        if (!isTimePaused)
        {
            // Gestion du temps de la journée
            dayTimer += Time.deltaTime;
            canvasManager.updateTimeSlider(dayTimer);

            // Calcul de la fraction du cycle (de 0 à 1)
            float cycleFraction = dayTimer / dayDuration;

            // Calcul de l'angle du soleil (de 0° à 360°)
            float sunAngle = cycleFraction * 360f;

            // Mise à jour de la rotation de la lumière directionnelle
            directionalLight.transform.rotation = Quaternion.Euler(new Vector3(sunAngle - 90f, 0f, 0f));

            // Ajustement de l'intensité de la lumière pour simuler le jour et la nuit
            float lightIntensity = Mathf.Clamp01(Mathf.Sin(cycleFraction * Mathf.PI));
            directionalLight.intensity = lightIntensity;

            // Optionnel : Ajustement de la couleur de la lumière pour simuler les levers et couchers de soleil
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
        Color noonColor = Color.white;                // Lumière blanche du midi
        Color sunsetColor = new Color(1f, 0.5f, 0f);  // Orange du coucher de soleil
        Color nightColor = Color.black;               // Noir pour la nuit

        if (cycleFraction <= 0.25f) // Lever du soleil
        {
            float t = cycleFraction / 0.25f;
            directionalLight.color = Color.Lerp(nightColor, sunriseColor, t);
        }
        else if (cycleFraction <= 0.5f) // Journée
        {
            float t = (cycleFraction - 0.25f) / 0.25f;
            directionalLight.color = Color.Lerp(sunriseColor, noonColor, t);
        }
        else if (cycleFraction <= 0.75f) // Après-midi vers coucher du soleil
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
            // Pas assez de nourriture, des individus meurent aléatoirement
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
        // Déterminer la position de spawn (à adapter selon vos besoins)
        Vector3 spawnPosition = GetRandomSpawnPosition();
        Quaternion spawnRotation = Quaternion.identity;

        GameObject newIndividual = Instantiate(individualPrefab, spawnPosition, spawnRotation);

        // Ajouter le nouvel individu à la liste
        AiController aiController = newIndividual.GetComponent<AiController>();
        if (aiController != null)
        {
            individuals.Add(aiController);
            aiController.Initialize(); // Initialiser l'agent si nécessaire
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        // Implémentez votre logique pour déterminer une position de spawn appropriée
        // Par exemple, une position aléatoire dans une zone définie
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
        isTimePaused = true;
        Time.timeScale = 0f;

        // Notifier les agents AI
        OnTimePause?.Invoke(true);
    }

    public void ResumeTime()
    {
        isTimePaused = false;
        Time.timeScale = 1f;

        // Notifier les agents AI
        OnTimePause?.Invoke(false);
    }
}
