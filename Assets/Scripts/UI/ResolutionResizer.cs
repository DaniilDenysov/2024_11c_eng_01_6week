using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ResolutionResizer : MonoBehaviour
{
    void Start()
    {
        var rect = GetComponent<RectTransform>();
        
        Debug.Log("Resizer: Scale" + rect.transform.localScale);
        Debug.Log("Resizer: Width, height" + rect.sizeDelta);
    }
}
