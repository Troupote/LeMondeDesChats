using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.AI;

public class GeneratorScript : MonoBehaviour
{
	[SerializeField]
	private int nbEntity = 10;

	[SerializeField]
	private GameObject[] entityPrefab;

    void Start()
    {
        for (int i = 0; i < nbEntity; i++)
        {
            
            int randomIndex = Random.Range(0, entityPrefab.Length);
            randomIndex = i<5 ? i : randomIndex;
            Vector3 randomPosition = new Vector3(
                transform.position.x + Random.Range(-5.0f, 5.0f),
                transform.position.y,
                transform.position.z + Random.Range(-5.0f, 5.0f)
            );
            var obj = Instantiate(entityPrefab[randomIndex], randomPosition, Quaternion.identity, gameObject.transform).GetComponent<NavMeshAgent>();
            RessourcesGlobales.Instance.RegisterVillagerAlive(1);
            if (obj.tag == "Builder")
                RessourcesGlobales.Instance.RegisterBuilderAlive(1);
        }
    }
}
