using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class HUDArranger : MonoBehaviour
{
    [SerializeField] private float shift = 0.6f;
    private new Camera _camera;
    private RectTransform _rectTransform;
    private Vector3 _originalPosition;

    private void Awake()
    {
        _camera = Camera.main;
        _rectTransform = GetComponent<RectTransform>();
        _originalPosition = _rectTransform.localPosition;
    }
    
    private void OnEnable()
    {
        _rectTransform.localPosition = _originalPosition;
        
        Rect screenBounds = new Rect(0f, 0f, Screen.width, Screen.height);
        Vector3[] objectCorners = new Vector3[4];
        _rectTransform.GetWorldCorners(objectCorners);
        
        Vector3 topLeftCornerPosition = _camera.WorldToScreenPoint(objectCorners[1]);
        Vector3 topRightCornerPosition = _camera.WorldToScreenPoint(objectCorners[2]);

        bool isTopLeftCornerVisible = screenBounds.Contains(topLeftCornerPosition);
        bool isTopRightCornerVisible = screenBounds.Contains(topRightCornerPosition);
        
        float onScreenYPoint = screenBounds.y + screenBounds.height / 2;
        bool isLeftXPointIncluded = screenBounds.Contains(new Vector3(topLeftCornerPosition.x, onScreenYPoint, 0));
        bool isRightXPointIncluded = screenBounds.Contains(new Vector3(topRightCornerPosition.x, onScreenYPoint, 0));
        bool isYPointIncluded = isTopLeftCornerVisible || isTopRightCornerVisible;

        if (!isYPointIncluded)
        {
            _rectTransform.localPosition = new Vector3(_rectTransform.localPosition.x,
                -_rectTransform.localPosition.y, 0);
        }
        
        if (!isLeftXPointIncluded)
        {
            _rectTransform.localPosition = new Vector3(_rectTransform.localPosition.x + shift,
                _rectTransform.localPosition.y, 0);
        } else if (!isRightXPointIncluded)
        {
            _rectTransform.localPosition = new Vector3(_rectTransform.localPosition.x - shift,
                _rectTransform.localPosition.y, 0);
        }
    }
}
