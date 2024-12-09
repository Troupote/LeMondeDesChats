using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using static UnityEditor.PlayerSettings;

public class DestroyManager: MonoBehaviour
{
    private GameObject aiReferences;
    [SerializeField] private GameObject studiantPrefab;
    private GameObject studiantPrefabCopy;
    private GameObject futurePrefab;

    private static DestroyManager destroyManager;

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
        
    }

    public void CollectDatas(GameObject aiSelected,GameObject prefabToInstantiate)
    {
        var aiController = aiSelected.GetComponent<AiController>();
        futurePrefab = prefabToInstantiate;
        Debug.Log(futurePrefab.name);
        aiReferences = aiSelected;
        var age = aiController.age;
        var pos = aiSelected.transform.position;
        Destroy(aiReferences);
        studiantPrefabCopy = Instantiate(studiantPrefab, pos, Quaternion.identity);
        var studiantController = studiantPrefabCopy.GetComponent<AiController>();
        studiantController.age = age;
        studiantController.SchoolState();
        
        OnEnable();
    }

    private void ReplaceStudiantByNewJob()
    {
        var age = studiantPrefabCopy.GetComponent<AiController>().age;
        var pos = studiantPrefabCopy.transform.position;
        Destroy(studiantPrefabCopy);
        var obj = Instantiate(futurePrefab,pos, Quaternion.identity);
        obj.GetComponent<AiController>().age = age;
    }

}
