using UnityEngine;
using UnityEngine.UI;

public class PerlinNoiseGenerator : MonoBehaviour
{
    public Image targetImage; // Le composant Image où la texture sera appliquée
    public int textureWidth = 256; // Largeur de la texture
    public int textureHeight = 256; // Hauteur de la texture
    public float period = 20f; // La période du bruit Perlin, peut être ajustée pour changer la "densité"

    // Perlin Vectors, ici pour la démonstration, nous utilisons des vecteurs aléatoires
    private Vector2[,] perlinVectors;

    void Start()
    {
        // Générer la grille de vecteurs Perlin
        GeneratePerlinVectors();

        // Générer la texture et appliquer le bruit Perlin
        Texture2D noiseTexture = GeneratePerlinNoiseTexture();
        targetImage.sprite = Sprite.Create(noiseTexture, new Rect(0, 0, noiseTexture.width, noiseTexture.height), new Vector2(0.5f, 0.5f));
    }

    void GeneratePerlinVectors()
    {
        int gridWidth = Mathf.CeilToInt(textureWidth / period);
        int gridHeight = Mathf.CeilToInt(textureHeight / period);

        perlinVectors = new Vector2[gridHeight + 1, gridWidth + 1];

        for (int y = 0; y <= gridHeight; y++)
        {
            for (int x = 0; x <= gridWidth; x++)
            {
                // Générer des vecteurs aléatoires
                perlinVectors[y, x] = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            }
        }
    }

    float Dot(Vector2 grad, float x, float y)
    {
        return grad.x * x + grad.y * y;
    }

    float Lerp(float a, float b, float t)
    {
        return a * (1 - t) + b * t;
    }

    float Fade(float t)
    {
        return 6 * Mathf.Pow(t, 5) - 15 * Mathf.Pow(t, 4) + 10 * Mathf.Pow(t, 3);
    }

    float GetPerlinNoise(float x, float y)
    {
        // Calculer les coordonnées de la cellule
        int cellX = Mathf.FloorToInt(x / period);
        int cellY = Mathf.FloorToInt(y / period);

        // Calculer les coordonnées relatives dans la cellule
        float relativeX = (x - cellX * period) / period;
        float relativeY = (y - cellY * period) / period;

        // Application de la fonction Fade
        relativeX = Fade(relativeX);
        relativeY = Fade(relativeY);

        // Récupérer les vecteurs de gradient pour chaque coin de la cellule
        Vector2 topLeftGradient = perlinVectors[cellY, cellX];
        Vector2 topRightGradient = perlinVectors[cellY, cellX + 1];
        Vector2 bottomLeftGradient = perlinVectors[cellY + 1, cellX];
        Vector2 bottomRightGradient = perlinVectors[cellY + 1, cellX + 1];

        // Calculer les contributions de chaque coin
        float topLeftContribution = Dot(topLeftGradient, relativeX, relativeY);
        float topRightContribution = Dot(topRightGradient, relativeX - 1, relativeY);
        float bottomLeftContribution = Dot(bottomLeftGradient, relativeX, relativeY - 1);
        float bottomRightContribution = Dot(bottomRightGradient, relativeX - 1, relativeY - 1);

        // Interpolation des contributions
        float topLerp = Lerp(topLeftContribution, topRightContribution, relativeX);
        float bottomLerp = Lerp(bottomLeftContribution, bottomRightContribution, relativeX);
        float finalValue = Lerp(topLerp, bottomLerp, relativeY);

        return finalValue / (Mathf.Sqrt(2) / 2); // Normalisation
    }

    Texture2D GeneratePerlinNoiseTexture()
    {
        Texture2D texture = new Texture2D(textureWidth, textureHeight);

        // Remplir la texture avec des valeurs de bruit Perlin
        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                float value = GetPerlinNoise(x, y);
                // Mappez la valeur de bruit à une couleur (par exemple en gris)
                texture.SetPixel(x, y, new Color(value, value, value));
            }
        }

        texture.Apply(); // Appliquer les modifications
        return texture;
    }
}
