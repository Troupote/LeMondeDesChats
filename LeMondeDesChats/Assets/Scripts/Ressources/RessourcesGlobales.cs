using UnityEngine;

public class RessourcesGlobales : MonoBehaviour
{
    public static RessourcesGlobales Instance;

    public int nourriture = 0;

    public int properityValue;
    public int prosperityMax;

    public int nbrVillager = 0;
    public int nbrBuilder = 0;
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

    public void RegisterVillagerAlive(int value)
    {
        nbrVillager += value;
        if(nbrVillager <= 0)
        {
            canvasManager?.EndGame(false);
        }
    }

    public void RegisterBuilderAlive(int value)
    {
        nbrBuilder += value;
        canvasManager?.updateBuilderText(nbrBuilder);
        Debug.Log("FJGKLFJG");
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
        canvasManager?.updateStoneText(nourriture);
    }

    public void AddProsperity(int quantite)
    {
        properityValue += quantite;
        if(properityValue < 0)
        {
            properityValue = 0;
        }
        else if(properityValue >= 100)
        {
            properityValue = 100;
            canvasManager?.EndGame(true);
        }
        canvasManager?.updateProperityGauge(properityValue);
    }

    public static bool IsRessourcesAvailable(BuildingSO SO)
    {
        bool result = Instance.bois >= SO.Wood && Instance.pierre >= SO.Stone && BuildManager.HasEnoughBuilder(SO);
        return result;
    }

    public static void UseRessources(int wood, int stone)
    {
        Instance.bois -= wood;
        Instance.pierre -= stone;
    }

    public static void UseRessources(BuildingSO SO)
    {
        Instance.bois -= SO.Wood;
        Instance.pierre -= SO.Stone;
        Instance.AddProsperity(10);
    }
}
