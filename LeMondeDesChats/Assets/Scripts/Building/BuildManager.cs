using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    public static bool IsBuilding { get; private set; }
    public static BuildingSO Building { get; set; }

    [SerializeField] private bool _deactivateBuilding = true;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void StartBuilding(BuildingSO SO)
    {
        IsBuilding = true;
        Building = SO;
    }

    public void StopBuilding()
    {
        Debug.Log("Stop Building");
        IsBuilding = false;
        Building = null;
    }

    public static GameObject Build(Transform parent)
    {
        GameObject newInstance = Instantiate(Building.Prefab, parent);
        // consume resources
        if (Instance._deactivateBuilding)
            Instance.StopBuilding();

        return newInstance;
    }
}
