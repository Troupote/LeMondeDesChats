using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiSelector : MonoBehaviour
{
    [SerializeField] private Material outline;
    private MeshRenderer oldRenderer;

    public void ApplyOutiline(GameObject obj)
    {

        var meshRenderer = obj.GetComponent<MeshRenderer>();
        var Mats = new Material[2] { obj.GetComponent<MeshRenderer>().material , outline};
        if (oldRenderer != null)
        {
            oldRenderer.materials = new Material[1] { oldRenderer.materials[0] };
            Debug.Log("Apply");
        }
        meshRenderer.materials = Mats;
        oldRenderer = meshRenderer;
        
    }
}
