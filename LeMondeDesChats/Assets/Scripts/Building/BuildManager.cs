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
    private bool _hasEnoughBuilderToUpdate;

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
            Instance.StartCoroutine(Coroutine_StartBuilding(tile, builders));

            if (Instance._deactivateBuilding)
                Instance.StopBuilding();
        }
    }

    public static bool HasEnoughBuilder(out AiController[] builders)
    {
        builders = GameObject.FindGameObjectsWithTag(Instance._builderTag)
            .Select(x => x.GetComponent<AiController>())
            .Where(x => x.etatActuel != AiController.AiState.Travail)
            .ToArray();

        return builders.Length >= Building.Worker;
    }

    public static bool HasEnoughBuilder() => HasEnoughBuilder(out AiController[] builders);

    private static IEnumerator Coroutine_StartBuilding(Tile tile, AiController[] builders)
    {
        var building = Instantiate(Building.Prefab, tile.transform);
        building.SetActive(false);
        tile.SetBuilding(building);

        yield return new WaitUntil(() => builders.All(x => x.IsAtWorkDestination()));

        building.SetActive(true);
        RessourcesGlobales.UseRessources(Building);
    }
}
