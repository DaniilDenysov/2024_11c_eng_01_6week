using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera camera;
    [SerializeField, Range(0, 100)] private float zoomSpeed = 10f;
    [SerializeField,Range(0,100)] private float minZoom = 5f;       
    [SerializeField, Range(0, 100)] private float maxZoom = 30f;

    void Update()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        float newSize = camera.m_Lens.OrthographicSize - scrollInput * zoomSpeed;
        camera.m_Lens.OrthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
    }
}
