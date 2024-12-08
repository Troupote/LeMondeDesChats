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

    [SerializeField]
    private CanvasManager canvasManager;

    private void Start()
    {
        canvasManager.timeSlider.maxValue = dayDuration;
    }
    void Update()
    {
        // Gestion du temps de la journ�e
        dayTimer += Time.deltaTime;
        canvasManager.updateTimeSlider(dayTimer);

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
