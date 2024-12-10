using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.AI;
using Unity.VisualScripting;

public class SchoolController : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropDownJobs;
    [SerializeField] private AiSelector aiSelector;
    private GameObject prefabToInstantiate;
    [SerializeField] private DestroyManager destroyManager;
    [SerializeField] private GameObject panelSchool;

    [SerializeField]
    private GameObject[] entityPrefab;

    private void Start()
    {
        Button buttonSchool = panelSchool.GetComponent<Button>();
        buttonSchool.interactable = false;
        panelSchool.GetComponent<Image>().color = Color.gray;
    }
    public void SelectJob()
    {
        aiSelector.schoolText.text = GetTextBeforeColon(aiSelector.schoolText.text) + ": "+  dropDownJobs.options[dropDownJobs.value].text;
        foreach (var entity in entityPrefab)
        {
            if (entity.tag == dropDownJobs.options[dropDownJobs.value].text)
            {
                prefabToInstantiate = entity;
                break;
            }
        }

    }

    public void UnlockSchool()
    {
        Button buttonSchool = panelSchool.GetComponent<Button>();
        buttonSchool.interactable = true;
        panelSchool.GetComponent<Image>().color = Color.white;
    }
    public void GoToSchool()
    {
        var aiControllerSelected =  aiSelector.aiSelected.GetComponent<AiController>();
        Debug.Log(prefabToInstantiate);
        destroyManager.CollectDatas(aiSelector.aiSelected,prefabToInstantiate);

        if(Time.timeScale == 0f)
        {
            QueueActions.AddActions(aiControllerSelected.SchoolState);
        }
        else
        {
            aiControllerSelected.SchoolState();
        }
        
    }


    private string GetTextBeforeColon(string text)
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
