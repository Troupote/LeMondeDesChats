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
        IsBuilding = false;
        Building = null;
    }

    public static void TryBuild(Tile tile)
    {
        if (IsBuilding
            && tile.CanBuild
            && RessourcesGlobales.IsRessourcesAvailable(Building))
        {
            // Navmesh to build

            tile.SetBuilding(Instantiate(Building.Prefab, tile.transform));
            RessourcesGlobales.UseRessources(Building);

            if (Instance._deactivateBuilding)
                Instance.StopBuilding();
        }
    }
}
