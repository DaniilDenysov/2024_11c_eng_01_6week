using System;
using System.Collections;
using System.Collections.Generic;
using Characters;
using Characters.Skills;
using UnityEngine;
using Validation;

[RequireComponent(typeof(CharacterMovement))]
public class SporeHook : Skill
{
    private PathValidator pathValidator;
    private int _range = 2;

    void Awake()
    {
        pathValidator = GetComponent<CharacterMovement>().GetPathValidator();
    }

    public override void Activate(Action<bool> onSetUp)
    {
        base.Activate(onSetUp);
        
        Vector3 characterPosition = transform.position;
        List<Vector3> directions = CharacterMovement.GetAllDirections();

        List<Vector3> litPositions = new List<Vector3>();

        foreach (Vector3 direction in directions)
        {
            for (int distance = 1; distance < _range + 1; distance++)
            {
                Vector3 currentCell = characterPosition + direction * distance;
                Vector3 nextCell = characterPosition + direction * (distance + 1);

                if (!pathValidator.CanMoveTo(characterPosition, currentCell))
                {
                    break;
                }
                
                if (pathValidator.CanMoveTo(characterPosition, currentCell) &&
                    !pathValidator.CanMoveTo(characterPosition, nextCell))
                {
                    litPositions.Add(transform.position + direction);
                    break;
                }
            }
        }

        TileSelector.Instance.SetTilesLit(litPositions, OnDirectionChosen);
    }

    private void OnDirectionChosen(Vector3 chosenTile)
    {
        Vector3 characterPosition = transform.position;
        Vector3 direction = chosenTile - characterPosition;
        
        for (int distance = 1; distance < _range + 1; distance++)
        {
            Vector3 currentCell = characterPosition + direction * (distance);
            Vector3 nextCell = characterPosition + direction * (distance + 1);
                
            if (pathValidator.CanMoveTo(characterPosition, currentCell) &&
                !pathValidator.CanMoveTo(characterPosition, nextCell))
            {
                transform.position = currentCell;
                break;
            }
        }
        
        OnActivated();
    }

    public override bool IsActivatable()
    {
        Vector3 characterPosition = transform.position;
        List<Vector3> directions = CharacterMovement.GetAllDirections();
        
        foreach (Vector3 direction in directions)
        {
            for (int distance = 0; distance < _range; distance++)
            {
                Vector3 currentCell = characterPosition + direction * (distance + 1);
                Vector3 nextCell = characterPosition + direction * (distance + 2);

                if (!pathValidator.CanMoveTo(characterPosition, currentCell))
                {
                    break;
                }
                
                if (pathValidator.CanMoveTo(characterPosition, currentCell) &&
                    !pathValidator.CanMoveTo(characterPosition, nextCell))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
