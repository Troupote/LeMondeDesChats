using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool CanBuild => _canBuild && _building == null;

    [SerializeField] private Transform _pivot;
    [SerializeField] private bool _canBuild;

    [Header("Selection")]
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private Color _validBuildColor = Color.green;
    [SerializeField] private Color _invalidBuildColor = Color.red;

    private GameObject _building;
    private Color _baseColor;

    private void Awake()
    {
        _baseColor = _renderer.material.color;
    }

    public void OnClick()
    {
        BuildManager.TryBuild(this);
    }

    public void OnHover(bool hovered)
    {
        if (!BuildManager.IsBuilding)
        {
            _renderer.material.color = _baseColor;
            return;
        }

        if (hovered)
        {
            if (CanBuild)
                _renderer.material.color = _validBuildColor;
            else
                _renderer.material.color = _invalidBuildColor;
        }
        else
        {
            _renderer.material.color = _baseColor;
        }
    }

    public void SetBuilding(GameObject building) => _building = building;
}
