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
    private string information;

    private void Start()
    {
        Button buttonSchool = panelSchool.GetComponent<Button>();
        buttonSchool.interactable = false;
        panelSchool.GetComponent<Image>().color = Color.gray;
        information = aiSelector.schoolText.text;
    }
    public void SelectJob()
    {
        aiSelector.schoolText.text = GetTextBeforeColon(information) + ": "+  dropDownJobs.options[dropDownJobs.value].text;
        foreach (var entity in entityPrefab)
        {
            if (entity.tag == dropDownJobs.options[dropDownJobs.value].text)
            {
                prefabToInstantiate = entity;
                break;
            }
        }
        Debug.Log("Drop" + prefabToInstantiate);


    }

    public void UnlockSchool()
    {
        Button buttonSchool = panelSchool.GetComponent<Button>();
        buttonSchool.interactable = true;
        panelSchool.GetComponent<Image>().color = Color.white;
    }
    public void GoToSchool()
    {
        if(prefabToInstantiate != null)
        {
            var aiControllerSelected = aiSelector.aiSelected.GetComponent<AiController>();
            Debug.Log(prefabToInstantiate);
            destroyManager.CollectDatas(aiSelector.aiSelected, prefabToInstantiate);

            if (Time.timeScale == 0f)
            {
                QueueActions.AddActions(aiControllerSelected.SchoolState);
            }
            else
            {
                aiControllerSelected.SchoolState();
            }
        }
        else
        {
            aiSelector.schoolText.text = "Select a job";
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
