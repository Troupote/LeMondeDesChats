using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public AgentController controller;
    public GridMaker gridMaker;
    private void OnTriggerEnter(Collider other)
    {
        var transformArray = new Transform[gridMaker.tilesPos.Length];
        for (int i = 0; i < gridMaker.tilesPos.Length; i++)
        {
            //Debug.Log(gridMaker.tilesPos[i].y);
            GameObject temp = new GameObject($"Transform_{i}");
            transformArray[i] = temp.transform;
            transformArray[i].position = new Vector3(gridMaker.tilesPos[i].x, 0, gridMaker.tilesPos[i].y);
            Destroy(temp);


        }
        controller.SetNextWaypoint(transformArray);
        //this.transform.position = transformArray[controller.currentWaypointIndex].position;
    }
}
