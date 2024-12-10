using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;
using static UnityEditor.PlayerSettings;

public class DestroyManager : MonoBehaviour
{
    private GameObject aiReferences; // Reference to the AI GameObject being managed
    [SerializeField] private GameObject studiantPrefab; // Prefab used to instantiate a studiant
    private GameObject studiantPrefabCopy; // Instance copy of the studiant prefab
    private GameObject futurePrefab; // Prefab to be used in the future job assignment
    [SerializeField] private GameObject locationToInstantiate; // Location where the prefabs will be instantiated

    private static DestroyManager destroyManager; // Singleton instance of DestroyManager

    /*
        public delegate void FinishingTrainingHandler();
        public static event FinishingTrainingHandler OnDestinationReached;

        private void OnEnable()
        {
            OnDestinationReached += ReplaceStudiantByNewJob; 
        }

        private void OnDisable()
        {
            OnDestinationReached -= ReplaceStudiantByNewJob; 
        }

        public static void TriggerDestinationReached()
        {
            // Invoke the event
            OnDestinationReached?.Invoke();
        }
        */
    private void Awake()
    {
        destroyManager = this; // Assign the current instance to the singleton reference
    }

    /// <summary>
    /// Collects data from the selected AI and instantiates a new studiant prefab.
    /// </summary>
    /// <param name="aiSelected">The selected AI GameObject.</param>
    /// <param name="prefabToInstantiate">The prefab to instantiate as the new job.</param>
    public void CollectDatas(GameObject aiSelected, GameObject prefabToInstantiate)
    {
        var aiController = aiSelected.GetComponent<AiController>();
        futurePrefab = prefabToInstantiate;
        aiReferences = aiSelected;
        var age = aiController.age;
        var pos = aiSelected.transform.position;
        destroyManager.studiantPrefabCopy = Instantiate(studiantPrefab, pos, Quaternion.identity, locationToInstantiate.transform);
        Destroy(aiReferences);

        Debug.Log("Prefab instantiated: " + destroyManager.studiantPrefabCopy);
        var studiantController = studiantPrefabCopy.GetComponent<AiController>();

        studiantController.age = age;
        studiantController.SchoolState();
    }

    /// <summary>
    /// Replaces the studiant with a new job by instantiating the future prefab.
    /// </summary>
    public static void ReplaceStudiantByNewJob()
    {
        var age = destroyManager.studiantPrefabCopy.GetComponent<AiController>().age;
        var pos = destroyManager.studiantPrefabCopy.transform.position;
        var obj = Instantiate(destroyManager.futurePrefab, pos, Quaternion.identity, destroyManager.locationToInstantiate.transform);
        obj.GetComponent<AiController>().age = age;
        Destroy(destroyManager.studiantPrefabCopy);
    }
}
