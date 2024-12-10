using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;
using static UnityEditor.PlayerSettings;

public class DestroyManager: MonoBehaviour
{
    private GameObject aiReferences;
    [SerializeField] private GameObject studiantPrefab;
    private GameObject studiantPrefabCopy;
    private GameObject futurePrefab;
    [SerializeField] private GameObject locationToInstantiate;

    private static DestroyManager destroyManager;

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
        // Invoquer l'événement
        OnDestinationReached?.Invoke();

    }*/

    public void CollectDatas(GameObject aiSelected,GameObject prefabToInstantiate)
    {
        var aiController = aiSelected.GetComponent<AiController>();
        futurePrefab = prefabToInstantiate;
        aiReferences = aiSelected;
        var age = aiController.age;
        var pos = aiSelected.transform.position;
        Destroy(aiReferences);

        // Instancier le prefab
        destroyManager.studiantPrefabCopy = Instantiate(studiantPrefab, pos, Quaternion.identity, locationToInstantiate.transform);
        Debug.Log("Prefab instancié : " + destroyManager.studiantPrefabCopy);
        var studiantController = studiantPrefabCopy.GetComponent<AiController>();

        studiantController.age = age;
        studiantController.SchoolState();
    }

    public static void ReplaceStudiantByNewJob()
    {
        
        var age = destroyManager.studiantPrefabCopy.GetComponent<AiController>().age;
        var pos = destroyManager.studiantPrefabCopy.transform.position;
        /*var obj = Instantiate(destroyManager.futurePrefab, pos, Quaternion.identity, destroyManager.locationToInstantiate.transform);
        obj.GetComponent<AiController>().age = age;*/
        Destroy(destroyManager.studiantPrefabCopy);
    }

}
