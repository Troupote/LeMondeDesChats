using UnityEngine;

public class RessourcesGlobales : MonoBehaviour
{
    public static RessourcesGlobales Instance;

    public int nourriture = 0;
    [field: SerializeField] public int bois { get; private set; } = 0;
    [field: SerializeField] public int pierre { get; private set; } = 0;

    [SerializeField]
    private CanvasManager canvasManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AjouterNourriture(int quantite)
    {
        nourriture += quantite;
        canvasManager?.updatefoodText(nourriture);
    }

    public void AjouterBois(int quantite)
    {
        bois += quantite;
        canvasManager?.updateWoodText(bois);
    }

    public void AjouterPierre(int quantite)
    {
        pierre += quantite;
    }

    public static bool IsRessourcesAvailable(BuildingSO SO) => Instance.bois >= SO.Wood && Instance.pierre >= SO.Stone;

    public static void UseRessources(int wood, int stone)
    {
        Instance.bois -= wood;
        Instance.pierre -= stone;
    }

    public static void UseRessources(BuildingSO SO)
    {
        Instance.bois -= SO.Wood;
        Instance.pierre -= SO.Stone;
    }
}
