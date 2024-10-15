using System;
using System.Collections.Generic;
using Characters;
using Characters.Skills;
using Ganeral;
using Managers;
using UnityEngine;
using Validation;

[RequireComponent(typeof(CharacterMovement))]
public class Jump : Skill
{
    private CharacterMovement movement;

    void Awake()
    {
        movement = GetComponent<CharacterMovement>();
    }

    public override void Activate(Action<bool> onSetUp)
    {
        base.Activate(onSetUp);
        
        PathValidator pathValidator = movement.GetPathValidator();
        Vector3 characterPosition = transform.position;
        List<Vector3> directions = CoordinateManager.GetAllDirections();

        List<Vector3> litPositions = new List<Vector3>();

        foreach (Vector3 direction in directions)
        {
            if (!pathValidator.CanMoveTo(characterPosition, characterPosition + direction))
            {
                litPositions.Add(characterPosition + direction);
            }
        }

        TileSelector.Instance.SetTilesLit(litPositions, OnCellChosen);
    }

    private void OnCellChosen(Vector3 chosenTile)
    {
        movement.Teleport(chosenTile);

        OnActivated();
    }

    public override bool IsActivatable()
    {
        PathValidator pathValidator = movement.GetPathValidator();
        Vector3 characterPosition = transform.position;
        List<Vector3> directions = CoordinateManager.GetAllDirections();

        foreach (Vector3 direction in directions)
        {
            if (!pathValidator.CanMoveTo(characterPosition, characterPosition + direction))
            {
                return true;
            }
        }

        return false;
    }
}