using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraScript : MonoBehaviour
{

    [SerializeField]
    private int speed = 5;

    [SerializeField]
    private int border = 20;

    private CameraControls CamInputs;
    
    // Start is called before the first frame update
    void Start()
    {
        CamInputs = new CameraControls();
        CamInputs.CameraControlMap.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveInput = CamInputs.CameraControlMap.Movements.ReadValue<Vector2>();
        Vector2 mousePosition = CamInputs.CameraControlMap.MouseNearBorders.ReadValue<Vector2>();

        transform.position += new Vector3(moveInput.x, 0 , moveInput.y) * Time.deltaTime * speed;

        if (Mathf.Abs(mousePosition.x) < border)
        {
            transform.position += Vector3.left * Time.deltaTime * speed;
        }
        if ((Mathf.Abs(mousePosition.x) > Screen.width - border) && Mathf.Abs(mousePosition.x) < Screen.width + border)
        {
            transform.position += Vector3.right * Time.deltaTime * speed;
        }
        if (Mathf.Abs(mousePosition.y) < border)
        {
            transform.position += Vector3.back * Time.deltaTime * speed;
        }
        if ((Mathf.Abs(mousePosition.y) > Screen.height - border) && Mathf.Abs(mousePosition.y) < Screen.height + border)
        {
            transform.position += Vector3.forward * Time.deltaTime * speed;
        }

    }
}
