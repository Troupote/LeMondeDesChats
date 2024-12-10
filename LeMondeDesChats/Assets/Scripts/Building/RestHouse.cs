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
        var renderer = obj.GetComponent<MeshRenderer>();
        var ai = obj.GetComponent<AiController>();

        renderer.enabled = false;
        yield return new WaitForSeconds(3f);

        if (ai != null)
        {
            obj.GetComponent<MeshRenderer>().enabled = true;
            ai?.RestState();
            remainingRoom.Remove(ai);
        }
    }
}
