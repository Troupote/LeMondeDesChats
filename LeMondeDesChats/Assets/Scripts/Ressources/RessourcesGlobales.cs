using UnityEngine;

public class RessourcesGlobales : MonoBehaviour
{
    public static RessourcesGlobales Instance;

    public int nourriture = 0;
    [SerializeField]
    public int bois { get; private set; } = 0;
    public int pierre = 0;

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
    }

    public void AjouterBois(int quantite)
    {
        bois += quantite;
    }

    public void AjouterPierre(int quantite)
    {
        pierre += quantite;
    }
}
