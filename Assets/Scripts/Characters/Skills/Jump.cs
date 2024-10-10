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
    [SerializeField] private TileSelector directionSelector;
    private CharacterMovement movement;

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
            if (!pathValidator.CanMoveTo(characterPosition, characterPosition + direction))
            {
                litPositions.Add(characterPosition + direction);
            }
        }

        directionSelector.SetTilesLit(litPositions);
        EventManager.OnLitTileClick += OnDirectionChosen;
    }

    private void OnDirectionChosen(Vector3 chosenTile)
    {
        EventManager.OnLitTileClick -= OnDirectionChosen;

        movement.Teleport(chosenTile, true);

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