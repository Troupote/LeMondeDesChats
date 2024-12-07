using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool CanBuild => _building == null;

    [SerializeField] private Transform _pivot;

    private GameObject _building;

    public void OnClick()
    {
        if (BuildManager.IsBuilding && CanBuild /*&& assez de ressources*/)
        {
            _building = BuildManager.Build(_pivot);
        }
    }
}
