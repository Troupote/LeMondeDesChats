using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AiSelector : MonoBehaviour
{
    [SerializeField] private Material outline; 
    private MeshRenderer oldRenderer; 
    [SerializeField] private GameObject IaPanel; 
    [SerializeField] public TMP_Text schoolText;
    public GameObject aiSelected; 

    /// <summary>
    /// Applies an outline to the selected AI and removes the outline from the previously selected AI.
    /// </summary>
    /// <param name="obj">The AI GameObject to be selected and highlighted.</param>
    public void ApplyOutline(GameObject obj)
    {
        IaPanelSetup(obj); 
        aiSelected = obj;

        var meshRenderer = obj.GetComponent<MeshRenderer>();
        var Mats = new Material[2] { obj.GetComponent<MeshRenderer>().material, outline };

        if (oldRenderer != null)
        {
            oldRenderer.materials = new Material[1] { oldRenderer.materials[0] };
        }

        meshRenderer.materials = Mats;
        oldRenderer = meshRenderer; 
    }

    /// <summary>
    /// Sets up the AI panel with information about the selected AI.
    /// </summary>
    /// <param name="obj">The AI GameObject whose information is to be displayed.</param>
    private void IaPanelSetup(GameObject obj)
    {
        IaPanel.SetActive(true); 
        var controller = obj.GetComponent<AiController>();
        schoolText.text = $"{controller.tag} goes to School to become : Builder";
    }
}
