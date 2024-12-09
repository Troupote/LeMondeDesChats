using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolDetector : MonoBehaviour
{
    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);

        var aiController = collision.gameObject.GetComponent<AiController>();
        if (aiController != null)
        {
            Debug.Log("AiController found, current state: " + aiController.etatActuel);
            if (aiController.etatActuel == AiController.AiState.School)
            {
                Debug.Log("AiController is in School state, triggering event.");
                DestroyManager.TriggerDestinationReached();
            }
        }
        else
        {
            Debug.Log("No AiController found on the colliding object.");
        }
    }
}