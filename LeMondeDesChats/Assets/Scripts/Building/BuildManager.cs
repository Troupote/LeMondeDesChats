using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    public static bool IsBuilding { get; private set; }
    public static BuildingSO Building { get; set; }

    [SerializeField] private GameObject _buttonPrefab;
    [SerializeField] private BuildingSO[] _buildings;
    [SerializeField] private Transform _content;
    [SerializeField] private Tag _builderTag;
    [SerializeField] private bool _deactivateBuilding = true;

    private List<BuildButton> _buildButtons;
    private AiController[] _builders;
    private bool _hasEnoughBuilderComputed;

    struct BuildButton
    {
        public Button Button;
        public BuildingSO BuildingSO;

        public BuildButton(Button button, BuildingSO so)
        {
            Button = button;
            BuildingSO = so;
        }

        public void Update()
        {
            Button.interactable = RessourcesGlobales.IsRessourcesAvailable(BuildingSO);
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (_content == null || _buttonPrefab == null || _buildings.Length <= 0)
            return;

        _buildButtons = new List<BuildButton>();

        foreach (var item in _buildings)
        {
            var go = Instantiate(_buttonPrefab, _content);
            var button = go.GetComponent<Button>();
            button.onClick.AddListener(() => StartBuilding(item));
            _buildButtons.Add(new BuildButton(button, item));
        }
    }

    private void Update()
    {
        foreach (var button in _buildButtons)
            button.Update();
    }

    private void LateUpdate()
    {
        _hasEnoughBuilderComputed = false;
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
            && RessourcesGlobales.IsRessourcesAvailable(Building)
            && HasEnoughBuilder(out var builders))
        {
            Instance.StartCoroutine(Coroutine_StartBuilding(tile, Building, builders));

            if (Instance._deactivateBuilding)
                Instance.StopBuilding();
        }
    }

    public static bool HasEnoughBuilder(out AiController[] builders)
    {
        Instance._builders = GameObject.FindGameObjectsWithTag(Instance._builderTag)
            .Select(x => x.GetComponent<AiController>())
            .Where(y => y.etatActuel != AiController.AiState.Travail)
            .ToArray();

        //Instance._hasEnoughBuilderComputed = true;
        
        builders = Instance._builders;

        Debug.Log($"HasEnoughBuilder : {builders.Length}|{Building.Worker} = {Instance._builders.Length >= Building.Worker}");
        return Instance._builders.Length >= Building.Worker;
    }

    public static bool HasEnoughBuilder() => HasEnoughBuilder(out AiController[] builders);

    private static IEnumerator Coroutine_StartBuilding(Tile tile, BuildingSO SO, AiController[] builders)
    {
        var building = Instantiate(SO.Prefab, tile.transform);
        Debug.Log("Spawn Prefab");
        building.SetActive(false);
        Debug.Log("Set Instance to deactivated");
        tile.SetBuilding(building);

        Debug.Log("Send GoBuild()");
        foreach (var builder in builders)
        {
            builder.GoBuild(tile.transform);
        }

        yield return new WaitUntil(() =>
        {
            var isAtWorkDestination = builders.All(x => x.IsAtWorkDestination());
            Debug.Log($"All isAtWorkDestination: {isAtWorkDestination}");
            return isAtWorkDestination;
        });
        Debug.Log("All reached destination");

        building.SetActive(true);
        Debug.Log("Set Instance to activated");
        RessourcesGlobales.UseRessources(SO);

        Debug.Log("Set back to wander");
        foreach (var builder in builders)
        {
            builder.etatActuel = AiController.AiState.Repos;
        }
    }
}
