using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    [SerializeField]
    private int speed;

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

        transform.position += new Vector3(moveInput.x, 0 , moveInput.y) * Time.deltaTime * speed;
    }
}
