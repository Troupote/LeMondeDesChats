using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildButtonHandler : MonoBehaviour
{
    [SerializeField] private Button _button;
    public BuildingSO BuildingSO;

    private void Update()
    {
        if (_button != null)
            _button.interactable = RessourcesGlobales.IsRessourcesAvailable(BuildingSO);
    }
}
