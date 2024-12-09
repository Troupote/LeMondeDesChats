using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CanvasManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text foodText ;

    [SerializeField]
    private TMP_Text woodText;

    [SerializeField]
    private TMP_Text stoneText;

    public Slider timeSlider;

    public Slider prosperityGauge;

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


    public void updateProperityGauge(float value)
    {
        prosperityGauge.value = value;
    }
}