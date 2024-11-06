using System;
using Managers;
using UnityEngine;

public class ScreenDetector : MonoBehaviour
{
    private void OnRectTransformDimensionsChange()
    {
        EventManager.FireEvent(EventManager.OnScreenSizeChanged);
    }
}