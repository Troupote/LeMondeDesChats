using UnityEngine;

public class CanvasBillboard : MonoBehaviour
{
    [SerializeField] private new Transform camera;

	private void Awake()
	{
		camera ??= Camera.main.transform;
	}

	private void LateUpdate()
	{
        transform.LookAt(transform.position + camera.forward);
	}
}