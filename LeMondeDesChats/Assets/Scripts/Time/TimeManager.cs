using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("Time Settings")]
    public float dayDuration = 60f;
    private float dayTimer = 0f;
    public int currentDay = 0;

    public float timeScale;

    [Header("Population Settings")]
    public float spawnInterval = 30f;
    private float spawnTimer = 0f;
    public GameObject[] individualPrefab;
    private List<AiController> individuals = new List<AiController>();

    [SerializeField]
    private CanvasManager canvasManager;

    [SerializeField] private GameObject locationToInstantiate;

    [SerializeField] private Light directionalLight;

    private bool isTimePaused = false;

    /// <summary>
    /// Delegate and event for handling time pause notifications
    /// </summary>
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
            dayTimer += Time.deltaTime;
            canvasManager.updateTimeSlider(dayTimer);

            float cycleFraction = dayTimer / dayDuration;
            float sunAngle = cycleFraction * 360f;
            directionalLight.transform.rotation = Quaternion.Euler(new Vector3(sunAngle - 90f, 0f, 0f));

            float lightIntensity = Mathf.Clamp01(Mathf.Sin(cycleFraction * Mathf.PI));
            directionalLight.intensity = lightIntensity;

            UpdateLightColor(cycleFraction);

            if (dayTimer >= dayDuration)
            {
                dayTimer = 0f;
                currentDay++;
                EndOfDay();
            }

            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval)
            {
                spawnTimer = 0f;
                SpawnNewIndividual();
            }
        }
    }

    /// <summary>
    /// Updates the color of the directional light based on the current cycle fraction to simulate different times of day.
    /// </summary>
    /// <param name="cycleFraction">The current fraction of the day cycle (0 to 1).</param>
    void UpdateLightColor(float cycleFraction)
    {
        Color sunriseColor = new Color(1f, 0.5f, 0f);
        Color noonColor = Color.white;
        Color sunsetColor = new Color(1f, 0.5f, 0f);
        Color nightColor = Color.black;

        if (cycleFraction <= 0.25f)
        {
            float t = cycleFraction / 0.25f;
            directionalLight.color = Color.Lerp(nightColor, sunriseColor, t);
        }
        else if (cycleFraction <= 0.5f)
        {
            float t = (cycleFraction - 0.25f) / 0.25f;
            directionalLight.color = Color.Lerp(sunriseColor, noonColor, t);
        }
        else if (cycleFraction <= 0.75f)
        {
            float t = (cycleFraction - 0.5f) / 0.25f;
            directionalLight.color = Color.Lerp(noonColor, sunsetColor, t);
        }
        else
        {
            float t = (cycleFraction - 0.75f) / 0.25f;
            directionalLight.color = Color.Lerp(sunsetColor, nightColor, t);
        }
    }

    /// <summary>
    /// Handles end-of-day activities such as aging individuals and managing food consumption.
    /// </summary>
    void EndOfDay()
    {
        foreach (AiController ai in individuals)
        {
            ai.AgeOneDay();
        }

        int totalIndividuals = individuals.Count;
        int availableFood = RessourcesGlobales.Instance.nourriture + RessourcesGlobales.Instance.farmProductions;
        if (availableFood >= totalIndividuals)
        {
            RessourcesGlobales.Instance.nourriture -= totalIndividuals;
        }
        else
        {
            int deficit = totalIndividuals - availableFood;
            RessourcesGlobales.Instance.nourriture = 0;

            for (int i = 0; i < deficit; i++)
            {
                if (individuals.Count > 0)
                {
                    int index = Random.Range(0, individuals.Count);
                    AiController starvingIndividual = individuals[index];
                    individuals.RemoveAt(index);
                    Destroy(starvingIndividual.gameObject);
                }
                else
                {
                    canvasManager?.EndGame(false);
                }
            }
        }
    }

    /// <summary>
    /// Spawns a new individual at a random position.
    /// </summary>
    void SpawnNewIndividual()
    {
        int randomIndex = Random.Range(0, individualPrefab.Length);
        Vector3 spawnPosition = GetRandomSpawnPosition();
        Quaternion spawnRotation = Quaternion.identity;

        GameObject newIndividual = Instantiate(individualPrefab[randomIndex], spawnPosition, spawnRotation, locationToInstantiate.transform);
        RessourcesGlobales.Instance.RegisterVillagerAlive(1);

        if (newIndividual.tag == "Builder")
        {
            RessourcesGlobales.Instance.RegisterBuilderAlive(1);
        }

        AiController aiController = newIndividual.GetComponent<AiController>();
        if (aiController != null)
        {
            individuals.Add(aiController);
            aiController.Initialize();
        }
    }

    /// <summary>
    /// Generates a random spawn position within defined bounds.
    /// </summary>
    /// <returns>A random Vector3 position for spawning.</returns>
    Vector3 GetRandomSpawnPosition()
    {
        return new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
    }

    /// <summary>
    /// Registers an AI controller to the list of individuals.
    /// </summary>
    /// <param name="ai">The AiController to register.</param>
    public void RegisterIndividual(AiController ai)
    {
        if (!individuals.Contains(ai))
            individuals.Add(ai);
    }

    /// <summary>
    /// Unregisters an AI controller from the list of individuals.
    /// </summary>
    /// <param name="ai">The AiController to unregister.</param>
    public void UnregisterIndividual(AiController ai)
    {
        if (individuals.Contains(ai))
            individuals.Remove(ai);
    }

    /// <summary>
    /// Pauses or resumes the game time and notifies AI agents of the pause state.
    /// </summary>
    public void PauseTime()
    {
        if (!isTimePaused)
        {
            isTimePaused = true;
            Time.timeScale = 0f;

            OnTimePause?.Invoke(true);
        }
        else
        {
            isTimePaused = false;
            Time.timeScale = 1f;

            OnTimePause?.Invoke(false);
            QueueActions.StartActions();
            QueueActions.ClearActions();
        }
    }
}
