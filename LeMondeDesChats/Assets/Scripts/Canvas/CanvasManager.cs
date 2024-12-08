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

    public Slider timeSlider;



    public void updatefoodText(int value)
    {
        foodText.text = value.ToString();
    }

    public void updateWoodText(int value)
    {
        woodText.text = value.ToString();
    }

    public void updateTimeSlider(float value)
    {
        timeSlider.value = value;
    }

}