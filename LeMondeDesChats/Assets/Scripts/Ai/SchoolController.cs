using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.AI;
using Unity.VisualScripting;

public class SchoolController : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropDownJobs; // Dropdown UI element for selecting jobs
    [SerializeField] private AiSelector aiSelector; // Reference to the AiSelector script
    private GameObject prefabToInstantiate; // Prefab selected to instantiate based on job choice
    [SerializeField] private DestroyManager destroyManager; // Reference to the DestroyManager script
    [SerializeField] private GameObject panelSchool; // Reference to the school panel UI

    [SerializeField]
    private GameObject[] entityPrefab; // Array of possible entity prefabs for different jobs
    private string information; // Stores initial information text from AiSelector

    private void Start()
    {
        Button buttonSchool = panelSchool.GetComponent<Button>();
        buttonSchool.interactable = false;
        panelSchool.GetComponent<Image>().color = Color.gray;
        information = aiSelector.schoolText.text;
    }

    /// <summary>
    /// Called when a job is selected from the dropdown.
    /// Updates the AI's selected job and sets the prefab to instantiate.
    /// </summary>
    public void SelectJob()
    {
        aiSelector.schoolText.text = GetTextBeforeColon(information) + ": " + dropDownJobs.options[dropDownJobs.value].text;

        foreach (var entity in entityPrefab)
        {
            if (entity.tag == dropDownJobs.options[dropDownJobs.value].text)
            {
                prefabToInstantiate = entity;
                break;
            }
        }
        Debug.Log("Selected Prefab: " + prefabToInstantiate);
    }

    /// <summary>
    /// Unlocks the school by enabling the school button and changing its color.
    /// </summary>
    public void UnlockSchool()
    {
        Button buttonSchool = panelSchool.GetComponent<Button>();
        buttonSchool.interactable = true;
        panelSchool.GetComponent<Image>().color = Color.white;
    }

    /// <summary>
    /// Sends the selected AI to school by instantiating the chosen job prefab.
    /// </summary>
    public void GoToSchool()
    {
        if (prefabToInstantiate != null)
        {
            var aiControllerSelected = aiSelector.aiSelected.GetComponent<AiController>();
            Debug.Log("Instantiating Prefab: " + prefabToInstantiate);

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

    /// <summary>
    /// Extracts and returns the substring before the first colon in the provided text.
    /// </summary>
    /// <param name="text">The input text to process.</param>
    /// <returns>The substring before the first colon, or the original text if no colon is found.</returns>
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
