using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SchoolController : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropDownJobs;
    [SerializeField] private AiSelector aiSelector;
    void GetWork()
    {
        GoToSchool();
        
    }
    public void SelectJob()
    {
        aiSelector.schoolText.text = GetTextBeforeColon(aiSelector.schoolText.text) + ": "+  dropDownJobs.options[dropDownJobs.value].text;
    }
    void GoToSchool()
    {

    }

    void AssignJob()
    {

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
