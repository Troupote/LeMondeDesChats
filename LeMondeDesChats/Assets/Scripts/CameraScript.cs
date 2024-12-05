using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;

public class CameraScript : MonoBehaviour
{

    [SerializeField]
    private int speed = 5;

    [SerializeField]
    private int rotateSpeed = 20;

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

        if (!Application.isFocused)
        {
            return;
        }

        Vector3 moveInput = CamInputs.CameraControlMap.Movements.ReadValue<Vector3>();
        Vector2 mousePosition = CamInputs.CameraControlMap.MouseNearBorders.ReadValue<Vector2>();


        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        transform.position += move * speed * Time.deltaTime;

        if (Mathf.Abs(mousePosition.x) < border)
        {
            transform.position += -transform.right * Time.deltaTime * speed;
        }
        if ((Mathf.Abs(mousePosition.x) > Screen.width - border) && Mathf.Abs(mousePosition.x) < Screen.width + border)
        {
            transform.position += transform.right * Time.deltaTime * speed;
        }
        if (Mathf.Abs(mousePosition.y) < border)
        {
            transform.position += -transform.forward * Time.deltaTime * speed;
        }
        if ((Mathf.Abs(mousePosition.y) > Screen.height - border) && Mathf.Abs(mousePosition.y) < Screen.height + border)
        {
            transform.position += transform.forward * Time.deltaTime * speed;
        }

        transform.Rotate(Vector3.up, moveInput.z * Time.deltaTime * rotateSpeed, Space.World);
        

    }
}
