using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshot : MonoBehaviour
{
    [SerializeField] private string _fileName = "New image";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DoScreenShot();
        }
    }

    [ContextMenu("ScreenShot")]
    public void DoScreenShot()
    {
        ScreenCapture.CaptureScreenshot($"{_fileName}.png");
        Debug.Log("Screenshot Saved");
    }
}
