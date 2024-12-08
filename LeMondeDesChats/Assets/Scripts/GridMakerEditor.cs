//using System;
using Unity.AI.Navigation;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;

public class GridMakerEditor : MonoBehaviour
{
    private Vector2[] tilesPos;
    [SerializeField] private int row;
    [SerializeField] private GameObject prefab;
    private Vector2 initialPos;
    //[SerializeField, Range(0.01f, 20f)] private float scale;

    private int index = 0;
    private NavMeshBaker navMeshBaker;
    //public PerlinNoiseTexture perlinNoiseTexture;
    [SerializeField] private Texture2D _heightTexture;
    [SerializeField] private Texture2D _zoneTexture;
    [SerializeField] private GameObject[] _tiles;
    [SerializeField] private Material[] _tileMaterials;

    [ContextMenu("Editor Generate Map")]
    private void EditorGenerate()
    {
        initialPos = new Vector2(0,0);
        int totalTiles = 1 + row * (row + 1) * 3 + row; 
        tilesPos = new Vector2[totalTiles];
        tilesPos[index++] = initialPos;
        navMeshBaker = GetComponent<NavMeshBaker>();

        TilesRegister();
        GenerateMap();
    }

    void TilesRegister()
    {
        Vector2[] NeighborOffsets = new Vector2[]
        {
            new Vector2(-2, 0),
            new Vector2(-1 , 1),
            new Vector2(1,1),
            new Vector2(2,0),
            new Vector2(1 ,-1),
            new Vector2(-1, -1)

        };
        for (int i = 1; i < row + 1; i++)
        {
            Vector2 transitionPos = initialPos + new Vector2(1, -1) * i;
             tilesPos[index++] = transitionPos;

            for (int j = 0; j < 6; j++)
            {
                for (int k = 0; k < i; k++)
                {
                    transitionPos += NeighborOffsets[j % 6];
                    tilesPos[index++] = transitionPos;

                }
            }

        }

        foreach (var elem in tilesPos)
        {
            //Debug.Log($"({elem.x},{elem.y})");
        }

        index = 0;
    }

    [ContextMenu("Destroy All Children")]
    void DestroyAllChildren()
    {
        for (int i = this.transform.childCount; i > 0; --i)
            DestroyImmediate(this.transform.GetChild(0).gameObject);

    }

    void GenerateMap()
    {
        DestroyAllChildren();

        float minX = 0; float minY = 0;
        var colorValues = new List<Vector2>();
        foreach (var elem in tilesPos)
        {
            minX = minX>elem.x ? elem.x : minX;
            minY = minX > elem.x ? elem.x : minX;
            colorValues.Add(elem);
        }

        foreach (var elem in tilesPos)
        {
            float xCoord = elem.x;
            float yCoord = elem.y;                                               
            float xCoordPerlin = (xCoord - minX);
            float yCoordPerlin = (yCoord - minY);

            //Vector2 randomVector2 = new Vector2(UnityEngine.Random.Range(0f, 100f), UnityEngine.Random.Range(0f, 100f));

            //perlinNoiseTexture.CreateTexture(perlinNoiseTexture.zoom, Vector2.zero,colorValues);
            //float noiseValue = perlinNoiseTexture.GeneratePerlinNoise(perlinNoiseTexture.zoom, Vector2.zero, new Vector2(xCoordPerlin, yCoordPerlin));
            float noiseValue = _heightTexture.GetPixel(Mathf.RoundToInt(xCoordPerlin), Mathf.RoundToInt(yCoordPerlin)).r ; // Get Texture Pixel;

            //Debug.Log(noiseValue);
            //Debug.Log($"({xCoord - minX},{yCoord - minY})");

            // -- Instatiate tile
            Vector3 position = new Vector3(xCoord * Mathf.Sqrt(3) / 2, noiseValue * 5, yCoord * 1.5f);
            float zoneValue = _zoneTexture.GetPixel(Mathf.RoundToInt(xCoordPerlin), Mathf.RoundToInt(yCoordPerlin)).r;
            GameObject prefabToSpawn/* = _tiles[Random.Range(0, _tiles.Length)]*/;
            // -- Assign color
            if (zoneValue < 0.25f) // Eau
                prefabToSpawn = _tiles[0];
            else if (zoneValue < 0.5f) // Plaines ou forêts
                prefabToSpawn = _tiles[1];
            else if (zoneValue < 0.75f) // Montagnes
                prefabToSpawn = _tiles[2];
            else // Sommets enneigés
                prefabToSpawn = _tiles[3];
            //GameObject obj = PrefabUtility.InstantiatePrefab(prefab, position + new Vector3(0,noiseValue*5,0), Quaternion.identity, this.transform);
            GameObject obj = PrefabUtility.InstantiatePrefab(prefabToSpawn, this.transform) as GameObject;
            obj.transform.position = position;
            Renderer renderer = obj.GetComponent<Renderer>();
            noiseValue = Mathf.Clamp01(noiseValue);
            //renderer.material.color = new Color(noiseValue, noiseValue, noiseValue);


        }
        //NavMeshSurface navMeshSurface = this.gameObject.AddComponent<NavMeshSurface>();
        //navMeshBaker.GenerateNavMesh(navMeshSurface);
    }

    Texture2D SpriteToTexture2D(Sprite sprite)
    {
        if (sprite == null)
        {
            Debug.LogError("Sprite is null!");
            return null;
        }

        // Obtenir les dimensions et les données de pixels du Sprite
        Rect spriteRect = sprite.rect;
        Texture2D originalTexture = sprite.texture;

        // Créer une nouvelle texture de la taille du Sprite
        Texture2D newTexture = new Texture2D((int)spriteRect.width, (int)spriteRect.height);

        // Copier les pixels de la texture originale
        Color[] pixels = originalTexture.GetPixels(
            (int)spriteRect.x,
            (int)spriteRect.y,
            (int)spriteRect.width,
            (int)spriteRect.height
        );
        newTexture.SetPixels(pixels);
        newTexture.Apply();

        return newTexture;
    }
}
 