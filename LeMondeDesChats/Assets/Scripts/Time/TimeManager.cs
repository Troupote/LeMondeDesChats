using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("Time Settings")]
    public float dayDuration = 60f; // Y unités de temps pour une journée
    private float dayTimer = 0f;
    public int currentDay = 0;

    [Header("Population Settings")]
    public float spawnInterval = 30f; // X unités de temps pour l'ajout de nouveaux individus
    private float spawnTimer = 0f;
    public GameObject individualPrefab; // Prefab de l'agent AI
    private List<AiController> individuals = new List<AiController>();

    void Update()
    {
        // Gestion du temps de la journée
        dayTimer += Time.deltaTime;
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

    void EndOfDay()
    {
        // Faire vieillir tous les individus
        foreach (AiController ai in individuals)
        {
            ai.AgeOneDay();
        }
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
}
