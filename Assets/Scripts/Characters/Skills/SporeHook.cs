using System.Collections;
using System.Collections.Generic;
using Characters;
using Characters.Skills;
using Ganeral;
using Managers;
using UnityEngine;
using Validation;

[RequireComponent(typeof(CharacterMovement))]
public class SporeHook : Skill
{
    [SerializeField] private TileSelector directionSelector;
    private CharacterMovement movement;
    private int _range = 2;

    void Awake()
    {
        movement = GetComponent<CharacterMovement>();
    }

    public override void Activate()
    {
        PathValidator pathValidator = movement.GetPathValidator();
        Vector3 characterPosition = transform.position;
        List<Vector3> directions = CoordinateManager.GetAllDirections();

        List<Vector3> litPositions = new List<Vector3>();

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
                    litPositions.Add(currentCell);
                    break;
                }
            }
        }

        directionSelector.SetTilesLit(litPositions, OnDirectionChosen);
    }

    private void OnDirectionChosen(Vector3 chosenTile)
    {
        movement.MakeCustomRotationMovement(chosenTile, true);
        OnActivated();
    }

    public override bool IsActivatable()
    {
        PathValidator pathValidator = movement.GetPathValidator();
        Vector3 characterPosition = transform.position;
        List<Vector3> directions = CoordinateManager.GetAllDirections();
        
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
