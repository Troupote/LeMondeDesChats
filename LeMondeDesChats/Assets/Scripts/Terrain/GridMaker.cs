using System;
using Unity.AI.Navigation;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.InputSystem.XR;

public class GridMaker : MonoBehaviour
{
    public Vector2[] tilesPos;
    [SerializeField] private int row;
    [SerializeField] private GameObject prefab;
    private Vector2 initialPos;
    //[SerializeField, Range(0.01f, 20f)] private float scale;

    private int index  = 0;
    private NavMeshBaker navMeshBaker;
    public PerlinNoiseTexture perlinNoiseTexture;

    public AgentController controller;
    public GridMaker gridMaker;
    public Waypoint waypoint;
    private NavMeshSurface navMeshSurface;


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
            float yCoordPerlin = (yCoord - minY) ;

            //Vector2 randomVector2 = new Vector2(UnityEngine.Random.Range(0f, 100f), UnityEngine.Random.Range(0f, 100f));

            perlinNoiseTexture.CreateTexture(perlinNoiseTexture.zoom, Vector2.zero,colorValues);
            float noiseValue = perlinNoiseTexture.GeneratePerlinNoise(perlinNoiseTexture.zoom, Vector2.zero, new Vector2(xCoordPerlin,yCoordPerlin));

            //Debug.Log(noiseValue);
            //Debug.Log($"({xCoord - minX},{yCoord - minY})");
            Vector3 position = new Vector3(xCoord * Mathf.Sqrt(3) / 2, 0, yCoord * 1.5f);
            GameObject obj = Instantiate(prefab, position + new Vector3(0,noiseValue*10,0), Quaternion.identity, this.transform);
            obj.transform.localScale = new Vector3(1,1,1);
            Renderer renderer = obj.GetComponent<Renderer>();
            noiseValue = Mathf.Clamp01(noiseValue);
            //renderer.material.color = new Color(noiseValue, noiseValue, noiseValue);

            if (noiseValue < 0.4f) // Eau
                renderer.material.color = new Color(0f, 0f, 0.8f); // Bleu
            else if (noiseValue < 0.6f) // Plaines ou forêts
                renderer.material.color = new Color(0.1f, 0.8f, 0.1f); // Vert
            else if (noiseValue < 0.8f) // Montagnes
                renderer.material.color = new Color(1f, 0.5f, 0f); // Gris
            else // Sommets enneigés
                renderer.material.color = new Color(1f, 0f, 1f); // Blanc

        }


        navMeshSurface = this.gameObject.AddComponent<NavMeshSurface>();
        navMeshBaker.GenerateNavMesh(navMeshSurface);


        var transformArray = new Transform[gridMaker.tilesPos.Length];
        for (int i = 0; i < gridMaker.tilesPos.Length; i++)
        {
            //Debug.Log(gridMaker.tilesPos[i].y);
            GameObject temp = new GameObject($"Transform_{i}");
            transformArray[i] = temp.transform;
            transformArray[i].position = new Vector3(gridMaker.tilesPos[i].x, 0, gridMaker.tilesPos[i].y);
            Destroy(temp);


        }
        //waypoint.transform.position = transformArray[controller.currentWaypointIndex].position;
        controller.SetNextWaypoint(transformArray);
        
    }
}
 