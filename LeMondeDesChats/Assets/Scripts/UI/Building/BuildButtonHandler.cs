using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BuildButtonHandler : MonoBehaviour
{
    private Button _button;
    public BuildingSO BuildingSO;

    private void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() => BuildManager.Instance.StartBuilding(BuildingSO));
    }

    private void Update()
    {
        if (_button != null)
            _button.interactable = RessourcesGlobales.IsRessourcesAvailable(BuildingSO);
    }
}
