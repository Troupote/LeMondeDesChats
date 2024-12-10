using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.AI;

public class GeneratorScript : MonoBehaviour
{
    [SerializeField]
    private int nbEntity = 10; // Number of entities to spawn

    [SerializeField]
    private GameObject[] entityPrefab; 

    void Start()
    {

        // For the first 5 entities, use a unique prefab based on the index
        // After that, continue selecting prefabs randomly
        for (int i = 0; i < nbEntity; i++)
        {
            int randomIndex = Random.Range(0, entityPrefab.Length);
            randomIndex = i < 5 ? i : randomIndex;

            Vector3 randomPosition = new Vector3(
                transform.position.x + Random.Range(-5.0f, 5.0f),
                transform.position.y,
                transform.position.z + Random.Range(-5.0f, 5.0f)
            );

            GameObject newEntity = Instantiate(entityPrefab[randomIndex], randomPosition, Quaternion.identity, gameObject.transform);

            NavMeshAgent agent = newEntity.GetComponent<NavMeshAgent>();

            RessourcesGlobales.Instance.RegisterVillagerAlive(1);

            // If the instantiated entity is tagged as "Builder", register it as a builder
            if (newEntity.tag == "Builder")
            {
                RessourcesGlobales.Instance.RegisterBuilderAlive(1);
            }
        }
    }
}
