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
    [SerializeField] private bool _deactivateBuilding = true;

    private List<BuildButton> _buildButtons;

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
