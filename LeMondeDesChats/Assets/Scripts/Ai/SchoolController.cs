using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.AI;

public class SchoolController : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropDownJobs;
    [SerializeField] private AiSelector aiSelector;
    private GameObject prefabToInstantiate;

    [SerializeField]
    private GameObject[] entityPrefab;
    public void SelectJob()
    {
        aiSelector.schoolText.text = GetTextBeforeColon(aiSelector.schoolText.text) + ": "+  dropDownJobs.options[dropDownJobs.value].text;
    }
    public void GoToSchool()
    {
        var aiControllerSelected =  aiSelector.aiSelected.GetComponent<AiController>();
        if(Time.timeScale == 0f)
        {
            QueueActions.AddActions(aiControllerSelected.SchoolState);
        }
        else
        {
            aiControllerSelected.SchoolState();
        }
        
    }

    public void AssignJob()
    {
        var aiControllerSelected = aiSelector.aiSelected.GetComponent<AiController>();
        var age = aiControllerSelected.age;
        var pos = aiSelector.aiSelected.transform.position;
        Destroy(aiSelector.aiSelected);
        foreach(var entity in entityPrefab) 
        {
            if(entity.tag == dropDownJobs.options[dropDownJobs.value].text)
            {
                prefabToInstantiate = entity;
                break;
            }

        }
        var obj = Instantiate(prefabToInstantiate,pos, Quaternion.identity, gameObject.transform).GetComponent<NavMeshAgent>();
        obj.GetComponent<AiController>().age = age;
    }

    string GetTextBeforeColon(string text)
    {
        int colonIndex = text.IndexOf(":");
        if (colonIndex != -1)
        {
            return text.Substring(0, colonIndex);
        }
        else
        {
            return text;
        }
    }
}
