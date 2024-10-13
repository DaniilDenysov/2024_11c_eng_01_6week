using System.Collections.Generic;
using Characters;
using Characters.Skills;
using Ganeral;
using UnityEngine;
using Validation;

[RequireComponent(typeof(CharacterMovement)), RequireComponent(typeof(Attack))]
public class LongTongue : Skill
{
    [SerializeField] private TileSelector cellSelector;
    private CharacterMovement _movement;
    private Attack _attack;
    private int _range = 2;

    void Awake()
    {
        _movement = GetComponent<CharacterMovement>();
        _attack = GetComponent<Attack>();
    }

    public override void Activate()
    {
        PathValidator pathValidator = _movement.GetPathValidator();
        Vector3 characterPosition = transform.position;

        List<Vector3> litPositions = new List<Vector3>();

        foreach (Vector3 direction in CoordinateManager.GetAttackDirections())
        {
            for (int distance = 0; distance < _range; distance++)
            {
                Vector3 currentCell = characterPosition + direction * (distance + 1);
                
                if (pathValidator.CanMoveTo(characterPosition, currentCell))
                {
                    foreach (GameObject entity in CoordinateManager.GetEntities(currentCell, gameObject))
                    {
                        if (entity.TryGetComponent(out Inventory _))
                        {
                            litPositions.Add(currentCell);
                        }
                    }
                }
            }
        }
        
        cellSelector.SetTilesLit(litPositions, OnCellChosen);
    }

    private void OnCellChosen(Vector3 chosenTile)
    {
        _attack.TryAttack(chosenTile);
        OnActivated();
    }

    public override bool IsActivatable()
    {
        PathValidator pathValidator = _movement.GetPathValidator();
        Vector3 characterPosition = transform.position;

        foreach (Vector3 direction in CoordinateManager.GetAttackDirections())
        {
            for (int distance = 0; distance < _range; distance++) {
                Vector3 currentCell = characterPosition + direction * (distance + 1);
                
                if (pathValidator.CanMoveTo(characterPosition, currentCell))
                {
                    foreach (GameObject entity in CoordinateManager.GetEntities(currentCell, gameObject))
                    {
                        if (entity.TryGetComponent(out Inventory _))
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }
}