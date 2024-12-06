using UnityEngine;

public class GridMaker : MonoBehaviour
{
    private Vector3[] tilesPos;
    [SerializeField] private int row;
    [SerializeField] private GameObject prefab;
    private Vector3 initialPos;


    private int index  = 0;



    private void Start()
    {
        initialPos = new Vector3(0,0,0);
        Instantiate(prefab,initialPos,Quaternion.identity);

        int totalTiles = 1 + row * (row + 1) * 3; 
        tilesPos = new Vector3[totalTiles];
        

        TilesPlacement();
    }

    void TilesPlacement()
    {
        Vector3[] NeighborOffsets = new Vector3[]
        {
            new Vector3(-(Mathf.Sqrt(3) / 2) * 2,0, 0),  
            new Vector3(-(Mathf.Sqrt(3) / 2) ,0, 1.5f),  
            new Vector3(Mathf.Sqrt(3) / 2,0,1.5f ),
            new Vector3((Mathf.Sqrt(3) / 2) * 2,0, 0),
            new Vector3((Mathf.Sqrt(3) / 2) ,0, -1.5f),
            new Vector3(-Mathf.Sqrt(3) / 2, 0, -1.5f)

        };
        for(int i = 1; i < row+1;i++)
        {
            Vector3 transitionPos = initialPos + new Vector3(Mathf.Sqrt(3) / 2, 0, -1.5f) * i;
            GameObject uil = Instantiate(prefab, transitionPos, Quaternion.identity);
            Renderer renderer = uil.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.Lerp(Color.red, Color.blue, (float)i / row);
            }
            for (int j = 0;j<6;j++)
            {
                for (int k = 0;k<i;k++)
                {
                    transitionPos += NeighborOffsets[j%6];
                    GameObject ui = Instantiate(prefab, transitionPos, Quaternion.identity);
                    ui.name = $"{i} + {j} + {k}";
                    Renderer rendererd = ui.GetComponent<Renderer>();
                    if (rendererd != null)
                    {
                        rendererd.material.color = Color.Lerp(Color.red, Color.blue, (float)i / row);
                    }

                }
            }

        }
    }


    void TilesRegister()
    {
        Vector3[] NeighborOffsets = new Vector3[]
        {
            new Vector3(-(Mathf.Sqrt(3) / 2) * 2,0, 0),
            new Vector3(-(Mathf.Sqrt(3) / 2) ,0, 1.5f),
            new Vector3(Mathf.Sqrt(3) / 2,0,1.5f ),
            new Vector3((Mathf.Sqrt(3) / 2) * 2,0, 0),
            new Vector3((Mathf.Sqrt(3) / 2) ,0, -1.5f),
            new Vector3(-Mathf.Sqrt(3) / 2, 0, -1.5f)

        };
        for (int i = 1; i < row + 1; i++)
        {
            Vector3 transitionPos = initialPos + new Vector3(Mathf.Sqrt(3) / 2, 0, -1.5f) * i;
            
            GameObject uil = Instantiate(prefab, transitionPos, Quaternion.identity);
            tilesPos[index++] = transitionPos;
            Renderer renderer = uil.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.Lerp(Color.red, Color.blue, (float)i / row);
            }
            for (int j = 0; j < 6; j++)
            {
                for (int k = 0; k < i; k++)
                {
                    transitionPos += NeighborOffsets[j % 6];
                    GameObject ui = Instantiate(prefab, transitionPos, Quaternion.identity);
                    tilesPos[index++] = transitionPos;
                    ui.name = $"{i} + {j} + {k}";
                    Renderer rendererd = ui.GetComponent<Renderer>();
                    if (rendererd != null)
                    {
                        rendererd.material.color = Color.Lerp(Color.red, Color.blue, (float)i / row);
                    }

                }
            }

        }
    }
}
