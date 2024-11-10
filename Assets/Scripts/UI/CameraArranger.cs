using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Camera))]
public class CameraArranger : MonoBehaviour
{
    [SerializeField] private Tilemap map;
    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void Start()
    {
        map.CompressBounds();
        ArrangeCamera();
        EventManager.OnScreenSizeChanged += ArrangeCamera;
    }

    private void ArrangeCamera()
    {
        float cameraHalfWidth = _camera.orthographicSize * _camera.aspect;
        float mapLeftEdgePosition = map.transform.position.x - (float)map.size.x / 2;

        transform.position = new Vector3(cameraHalfWidth + mapLeftEdgePosition, 
            transform.position.y, transform.position.z);
    }
}
