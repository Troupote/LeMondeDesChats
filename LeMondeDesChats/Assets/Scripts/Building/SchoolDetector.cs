using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolDetector : MonoBehaviour
{
    public void OnTriggerEnter(Collider collision)
    {

        var aiController = collision.gameObject.GetComponent<AiController>();
        if (aiController != null)
        {

            if (aiController.etatActuel == AiController.AiState.School)
            {
                Debug.Log("AiController is in School state, triggering event.");
                DestroyManager.TriggerDestinationReached();
            }
        }
    }
}