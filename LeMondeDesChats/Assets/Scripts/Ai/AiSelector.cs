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
    [HideInInspector] public GameObject aiSelected;

    public void ApplyOutline(GameObject obj)
    {
        IaPanelSetup(obj);
        aiSelected = obj;
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
    private void IaPanelSetup(GameObject obj)
    {
        IaPanel.SetActive(true);
        var controller = obj.GetComponent<AiController>();
        schoolText.text = $"{controller.tag} goes to School to become : Builder";
    }

}
