using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GetSelector : MonoBehaviour
{
    
    public void FindObject()
    {
        var obj = GameObject.Find("GetSelector");
        obj.GetComponent<AiSelector>().ApplyOutline(this.gameObject);
    }


}
