using Collectibles;
using CustomTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : MonoBehaviour, ICollectible
{
    [SerializeField, ReadOnly] private int currentPoints; 
    private int[] points = { 2, 4, 6 };

    private void Awake()
    {
        currentPoints = points[Random.Range(0, points.Length)];
    }

    public object Collect()
    {
        gameObject.SetActive(false);
        return this;
    }
}
