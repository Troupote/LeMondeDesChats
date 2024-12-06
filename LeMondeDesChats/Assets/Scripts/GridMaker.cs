using System;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.UIElements;

public class GridMaker : MonoBehaviour
{
    private Vector2[] tilesPos;
    [SerializeField] private int row;
    [SerializeField] private GameObject prefab;
    private Vector2 initialPos;
    //[SerializeField, Range(0.01f, 20f)] private float scale;

    private int index  = 0;
    private NavMeshBaker navMeshBaker;
    public PerlinNoiseTexture perlinNoiseTexture;


    private void Start()
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
    void GenerateMap()
    {
        foreach (var elem in tilesPos)
        {
            float xCoord = elem.x;
            float yCoord = elem.y;                                               
            float xCoordPerlin = (xCoord + row) / 2 * row;
            float yCoordPerlin = (yCoord + row) / 2 * row;

            Vector2 randomVector2 = new Vector2(UnityEngine.Random.Range(0f, 100f), UnityEngine.Random.Range(0f, 100f));

            perlinNoiseTexture.CreateTexture(perlinNoiseTexture.zoom, randomVector2);
            float noiseValue = perlinNoiseTexture.GeneratePerlinNoise(perlinNoiseTexture.zoom,randomVector2,new Vector2(xCoord,yCoord));

            //Debug.Log(noiseValue);
            Debug.Log($"({xCoord},{yCoord})");
            Vector3 position = new Vector3(xCoord * Mathf.Sqrt(3) / 2, 0, yCoord * 1.5f);
            GameObject obj = Instantiate(prefab, position, Quaternion.identity, this.transform);
            Renderer renderer = obj.GetComponent<Renderer>();

            if (noiseValue < 0.3f) 
            {
                renderer.material.color = Color.yellow;
            }
            else if (noiseValue < 0.7f)
            {
                renderer.material.color = Color.green;
            }
            else
            {
                renderer.material.color = Color.red;
            }
        }
        NavMeshSurface navMeshSurface = this.gameObject.AddComponent<NavMeshSurface>();
        navMeshBaker.GenerateNavMesh(navMeshSurface);
    }
}
 