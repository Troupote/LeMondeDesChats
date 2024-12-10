using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestHouse : MonoBehaviour
{
    public List<AiController> remainingRoom ;


    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<AiController>() != null)
        {
            if (other.GetComponent<AiController>().currentState == AiController.AiState.Rest)
            {
                if (remainingRoom.Count < 5)
                {
                    StartCoroutine(ActiveRenderer(other.gameObject));
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
        remainingRoom.Remove(obj.GetComponent<AiController>());

    }
}
