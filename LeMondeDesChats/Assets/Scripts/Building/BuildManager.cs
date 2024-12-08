using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static bool IsBuilding { get; private set; }
    public static BuildingSO Building { get; set; }

    [SerializeField] private bool _deactivateBuilding = true;

    public void StartBuilding(BuildingSO SO)
    {
        IsBuilding = true;
        Building = SO;
    }

    public void StopBuilding()
    {
        if (_deactivateBuilding)
            IsBuilding= false;
        
        Building = null;
    }

    public static GameObject Build(Transform parent)
    {
        // consume resources
        return Instantiate(Building.Prefab, parent);
    }
}
