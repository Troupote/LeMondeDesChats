using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolDetector : MonoBehaviour
{
    public void OnTriggerEnter(Collision collision)
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
    }
}