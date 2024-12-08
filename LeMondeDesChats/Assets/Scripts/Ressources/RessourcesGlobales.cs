using UnityEngine;

public class RessourcesGlobales : MonoBehaviour
{
    public static RessourcesGlobales Instance;

    public int nourriture = 0;
    [SerializeField]
    public int bois { get; private set; } = 0;
    public int pierre = 0;

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
        canvasManager.updatefoodText(nourriture);
    }

    public void AjouterBois(int quantite)
    {
        bois += quantite;
        canvasManager.updateWoodText(bois);
    }

    public void AjouterPierre(int quantite)
    {
        pierre += quantite;
    }
}
