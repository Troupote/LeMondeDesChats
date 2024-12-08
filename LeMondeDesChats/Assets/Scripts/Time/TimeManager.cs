using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("Time Settings")]
    public float dayDuration = 60f; // Y unit�s de temps pour une journ�e
    private float dayTimer = 0f;
    public int currentDay = 0;

    [Header("Population Settings")]
    public float spawnInterval = 30f; // X unit�s de temps pour l'ajout de nouveaux individus
    private float spawnTimer = 0f;
    public GameObject individualPrefab; // Prefab de l'agent AI
    private List<AiController> individuals = new List<AiController>();

    void Update()
    {
        // Gestion du temps de la journ�e
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
        // D�terminer la position de spawn (� adapter selon vos besoins)
        Vector3 spawnPosition = GetRandomSpawnPosition();
        Quaternion spawnRotation = Quaternion.identity;

        GameObject newIndividual = Instantiate(individualPrefab, spawnPosition, spawnRotation);

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
}
