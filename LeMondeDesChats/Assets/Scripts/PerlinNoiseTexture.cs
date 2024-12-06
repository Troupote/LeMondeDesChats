using UnityEngine;
using UnityEngine.UI;

public class PerlinNoiseTexture : MonoBehaviour
{
    public int textureWidth = 256; // Largeur de la texture
    public int textureHeight = 256; // Hauteur de la texture
    [Range(0.01f, 10f)] public float zoom = 10f; // Facteur de zoom pour le bruit de Perlin
    private Vector2 offset = new Vector2(0, 0); // Décalage de la texture

    public Image image;

    void Start()
    {

    }

    public void CreateTexture(float zoom, Vector2 offset)
    {
        Texture2D texture = GeneratePerlinNoiseTexture();
        image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    Texture2D GeneratePerlinNoiseTexture()
    {
        Texture2D texture = new Texture2D(textureWidth, textureHeight);

        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                float xCoord = (float)x / textureWidth * zoom + offset.x;
                float yCoord = (float)y / textureHeight * zoom + offset.y;
                float perlinValue = Mathf.PerlinNoise(xCoord, yCoord);
                perlinValue = Mathf.Clamp01(perlinValue);
                texture.SetPixel(x, y, new Color(perlinValue, perlinValue, perlinValue));
            }
        }

        texture.Apply();
        return texture;
    }

    public float GeneratePerlinNoise(float zoom, Vector2 offset,Vector2 pos)
    {

        float xCoord = (float)pos.x / textureWidth * zoom + offset.x;
        float yCoord = (float)pos.y / textureHeight * zoom + offset.y;
        float perlinValue = Mathf.PerlinNoise(xCoord, yCoord);
        perlinValue = Mathf.Clamp01(perlinValue);

        return perlinValue;
    }
}
