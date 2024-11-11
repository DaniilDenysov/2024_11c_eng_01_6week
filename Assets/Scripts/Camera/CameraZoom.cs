using Cinemachine;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera camera;
    [SerializeField, Range(0, 100)] private float zoomSpeed = 2f;
    [SerializeField, Range(0, 100)] private float minZoom = 5f;
    [SerializeField, Range(0, 100)] private float maxZoom = 30f;
    [SerializeField] private float zoomSmoothTime = 0.1f;

    private float targetZoom;
    private float zoomVelocity;

    void Start()
    {
        targetZoom = camera.m_Lens.OrthographicSize;
    }

    void Update()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scrollInput) > 0.01f)
        {
            targetZoom -= scrollInput * zoomSpeed;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }

        camera.m_Lens.OrthographicSize = Mathf.SmoothDamp(camera.m_Lens.OrthographicSize, targetZoom, ref zoomVelocity, zoomSmoothTime);
    }
}