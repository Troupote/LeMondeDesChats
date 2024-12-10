using UnityEngine;

public class RessourcesGlobales : MonoBehaviour
{
    public static RessourcesGlobales Instance; // Singleton instance

    public int nourriture = 0; // Current food resource

    public int properityValue; // Current prosperity value
    public int prosperityMax; // Maximum prosperity value

    public int farmProductions = 0; // Number of farm productions
    public int nbrVillager = 0; // Number of villagers alive
    public int nbrBuilder = 0; // Number of builders alive

    [field: SerializeField] public int bois { get; private set; } = 0; // Wood resource with private setter
    [field: SerializeField] public int pierre { get; private set; } = 0; // Stone resource with private setter

    [SerializeField]
    private CanvasManager canvasManager; // Reference to the CanvasManager for UI updates

    void Awake()
    {
        // Initialize the singleton instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }

    /// <summary>
    /// Registers a change in the number of alive villagers.
    /// </summary>
    /// <param name="value">The change in the number of villagers.</param>
    public void RegisterVillagerAlive(int value)
    {
        nbrVillager += value;
        if (nbrVillager <= 0)
        {
            canvasManager?.EndGame(false); // End game if no villagers remain
        }
    }

    /// <summary>
    /// Registers a change in the number of alive builders.
    /// </summary>
    /// <param name="value">The change in the number of builders.</param>
    public void RegisterBuilderAlive(int value)
    {
        nbrBuilder += value;
        canvasManager?.updateBuilderText(nbrBuilder); // Update builder count in UI
    }

    /// <summary>
    /// Adds or subtracts food resource.
    /// </summary>
    /// <param name="quantite">The amount of food to add.</param>
    public void AjouterNourriture(int quantite)
    {
        nourriture += quantite;
        canvasManager?.updatefoodText(nourriture); // Update food count in UI
    }

    /// <summary>
    /// Adds wood resource.
    /// </summary>
    /// <param name="quantite">The amount of wood to add.</param>
    public void AjouterBois(int quantite)
    {
        bois += quantite;
        canvasManager?.updateWoodText(bois); // Update wood count in UI
    }

    /// <summary>
    /// Adds stone resource.
    /// </summary>
    /// <param name="quantite">The amount of stone to add.</param>
    public void AjouterPierre(int quantite)
    {
        pierre += quantite;
        canvasManager?.updateStoneText(pierre); // Update stone count in UI
    }

    /// <summary>
    /// Adds or subtracts prosperity value and checks for game end conditions.
    /// </summary>
    /// <param name="quantite">The amount to change the prosperity value.</param>
    public void AddProsperity(int quantite)
    {
        properityValue += quantite;
        if (properityValue <= 0)
        {
            properityValue = 0;
            //canvasManager?.EndGame(false);
        }
        else if (properityValue >= 100)
        {
            properityValue = 100;
            canvasManager?.EndGame(true); // End game if prosperity reaches maximum
        }
        canvasManager?.updateProperityGauge(properityValue); // Update prosperity gauge in UI
    }

    /// <summary>
    /// Checks if the required resources are available for a building.
    /// </summary>
    /// <param name="SO">The building scriptable object containing resource requirements.</param>
    /// <returns>True if resources are available, otherwise false.</returns>
    public static bool IsRessourcesAvailable(BuildingSO SO)
    {
        bool result = Instance.bois >= SO.Wood && Instance.pierre >= SO.Stone && BuildManager.HasEnoughBuilder(SO);
        return result;
    }

    /// <summary>
    /// Uses the specified amount of wood and stone resources.
    /// </summary>
    /// <param name="wood">Amount of wood to use.</param>
    /// <param name="stone">Amount of stone to use.</param>
    public static void UseRessources(int wood, int stone)
    {
        Instance.bois -= wood;
        Instance.pierre -= stone;
    }

    /// <summary>
    /// Uses resources based on the building's requirements.
    /// </summary>
    /// <param name="SO">The building scriptable object containing resource requirements.</param>
    public static void UseRessources(BuildingSO SO)
    {
        Instance.bois -= SO.Wood;
        Instance.pierre -= SO.Stone;
    }
}
