using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestHouse : MonoBehaviour
{
    private int remainingRoom = 4;


    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<AiController>() != null)
        {
            if (other.GetComponent<AiController>().etatActuel == AiController.AiState.Repos)
            {
                if (remainingRoom > 0)
                {
                    remainingRoom--;
                    var objRenderer = other.GetComponent<MeshRenderer>();
                }
            }
        }


    }

    IEnumerator ActiveRenderer(GameObject obj)
    {
        obj.GetComponent<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(3f);
        obj.GetComponent<MeshRenderer>().enabled = true;
        obj.GetComponent<AiController>().RestState();

    }
}
