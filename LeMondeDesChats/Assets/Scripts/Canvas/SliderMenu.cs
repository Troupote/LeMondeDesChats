using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderMenu : MonoBehaviour
{
    public GameObject PanelMenu;

    public void ShowHideMenu()
    {
        Animator animator = GetComponent<Animator>();
        bool IsOpen = animator.GetBool("IsMenuActivated");
        animator.SetBool("IsMenuActivated", !IsOpen);
    }
}
