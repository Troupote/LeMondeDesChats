using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class CanvasManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text foodText ;

    [SerializeField]
    private TMP_Text woodText;

    [SerializeField]
    private TMP_Text stoneText;

    [SerializeField]
    private TMP_Text builderText;

    public Slider timeSlider;

    public Slider prosperityGauge;

    [SerializeField]
    private GameObject GameOverPanel;


    [SerializeField]
    private TMP_Text textGameOver;
    [SerializeField]
    private GameObject GamePanel;

    private void Start()
    {
        prosperityGauge.minValue = 0;
        prosperityGauge.maxValue = RessourcesGlobales.Instance.prosperityMax;
    }

    public void updatefoodText(int value)
    {
        foodText.text = value.ToString();
    }

    public void updateWoodText(int value)
    {
        woodText.text = value.ToString();
    }

    public void updateStoneText(int value)
    {
        stoneText.text = value.ToString();
    }

    public void updateTimeSlider(float value)
    {
        timeSlider.value = value;
    }
    public void updateBuilderText(int value)
    {
        builderText.text = $"Remaining builders : {value}";
    }



    public void updateProperityGauge(float value)
    {
        prosperityGauge.value = value;
    }

    public void EndGame(bool IsWon)
    {
        GameOverPanel.SetActive(true);
        GamePanel.SetActive(false);
        textGameOver.text = IsWon == true ? "GAME OVER" : "GG";
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}