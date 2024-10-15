using System;
using Cards;
using CustomTools;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TestAction : MonoBehaviour, ITurnAction
{
    [SerializeField, Range(0, 360f)] private float rotationStep = 90f;
    [SerializeField, ReadOnly] private Vector2Int directionNormolized;
    [SerializeField] private GameObject HUD_display;

    private void Awake()
    {
      directionNormolized = Vector2Int.left;
    }

    public void OnTurn()
    {
        HUD_display.SetActive(true);
    }

    public void ChooseNewDirection(Action onDirectionChosen)
    {
        throw new NotImplementedException();
    }

    public void RotateObject()
    {
        Vector2 directionContinuous = directionNormolized;
        Quaternion rotation = Quaternion.Euler(0, 0, rotationStep);
        directionContinuous = rotation * directionContinuous;
        directionNormolized = new Vector2Int(Mathf.RoundToInt(directionContinuous.x), Mathf.RoundToInt(directionContinuous.y));
    }

}
