using UnityEngine;

public class CursorController : MonoBehaviour
{
    private Camera camera;

    private void Start()
    {
        camera = Camera.main;
    }

    private void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = camera.ScreenToWorldPoint(mousePosition);
        mousePosition.z = 0;
        transform.position = mousePosition;
    }
}
