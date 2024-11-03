using System;
using System.Collections.Generic;
using Characters;
using Characters.Skills;
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

        PathValidator pathValidator = PathValidator.Instance;
        Vector3 characterPosition = transform.position;
        List<Vector3> directions = CharacterMovement.GetAllDirections();

        List<Vector3> litPositions = new List<Vector3>();

        foreach (Vector3 direction in directions)
        {
            Vector3 position = characterPosition + direction;
            
            if (!pathValidator.CanMoveTo(characterPosition, position)
                && !pathValidator.IsOutOfMap(position))
            {
                litPositions.Add(position);
            }
        }

        InputManager.Instance.AddCellCallbacks(new HashSet<Vector3>(litPositions), OnCellChosen);
    }

    private void OnCellChosen(Vector3 chosenTile)
    {
        transform.position = chosenTile;

        OnActivated();
    }

    public override bool IsActivatable()
    {
        PathValidator pathValidator = PathValidator.Instance;
        Vector3 characterPosition = transform.position;
        List<Vector3> directions = CharacterMovement.GetAllDirections();

        foreach (Vector3 direction in directions)
        {
            Vector3 position = characterPosition + direction;
            
            if (!pathValidator.CanMoveTo(characterPosition, position) 
                && !pathValidator.IsOutOfMap(position))
            {
                return true;
            }
        }

        return false;
    }
}