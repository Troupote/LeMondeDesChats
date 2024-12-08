using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBaker : MonoBehaviour
{
    public void GenerateNavMesh(NavMeshSurface surfaces)
    {
        surfaces.BuildNavMesh(); 
    }
}
