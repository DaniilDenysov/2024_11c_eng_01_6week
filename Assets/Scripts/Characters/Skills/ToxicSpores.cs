using System;
using System.Collections;
using System.Collections.Generic;
using Cards;
using Characters;
using Characters.Skills;
using Collectibles;
using Ganeral;
using Managers;
using UnityEngine;
using UnityEngine.Events;
using Validation;

[RequireComponent(typeof(CharacterMovement)),
 RequireComponent(typeof(Attack)),
 RequireComponent(typeof(ICollector))]
public class ToxicSpores : Skill
{
    [SerializeField] private TileSelector directionSelector;
    private CharacterMovement _movement;
    private Attack _attack;
    private ICollector _collector;
    private int _range = 2;

    void Awake()
    {
        _movement = GetComponent<CharacterMovement>();
        _collector = GetComponent<ICollector>();
        _attack = GetComponent<Attack>();
    }

    public override void Activate()
    {
        PathValidator pathValidator = _movement.GetPathValidator();
        Vector3 characterPosition = transform.position;
        List<Vector3> directions = CoordinateManager.GetAllDirections();

        List<Vector3> litPositions = new List<Vector3>();

        foreach (Vector3 direction in directions)
        {
            for (int distance = 0; distance < _range; distance++)
            {
                Vector3 currentCell = characterPosition + direction * (distance + 1);

                if (!pathValidator.CanMoveTo(characterPosition, currentCell))
                {
                    break;
                }
                
                foreach (GameObject entity in CoordinateManager.GetEntities(currentCell))
                {
                    if (entity.TryGetComponent(out Inventory _))
                    {
                        litPositions.Add(currentCell);
                    }
                    else if (entity.TryGetComponent(out ICollectible collectible))
                    {
                        if (collectible.GetType() == typeof(Human))
                        {
                            litPositions.Add(currentCell);
                        }
                    }
                }
            }
        }

        directionSelector.SetTilesLit(litPositions, OnDirectionChosen);
    }

    private void OnDirectionChosen(Vector3 chosenTile)
    {
        Vector3 characterPosition = transform.position;
        Vector3 direction = chosenTile - characterPosition;
        bool result = false;

        for (int i = 1; i < _range; i++)
        {
            Vector3 currentCell = characterPosition + direction * i;
            result = result || CoordinateManager.AttackAndEatAtCell(currentCell, _attack, _collector);
        }

        OnActivated(result);
    }

    public override bool IsActivatable()
    {
        PathValidator pathValidator = _movement.GetPathValidator();
        Vector3 characterPosition = transform.position;
        List<Vector3> directions = CoordinateManager.GetAllDirections();
        
        foreach (Vector3 direction in directions)
        {
            for (int distance = 0; distance < _range; distance++)
            {
                Vector3 currentCell = characterPosition + direction * (distance + 1);

                if (!pathValidator.CanMoveTo(characterPosition, currentCell))
                {
                    break;
                }
                
                foreach (GameObject entity in CoordinateManager.GetEntities(currentCell))
                {
                    if (entity.TryGetComponent(out Inventory _))
                    {
                        return true;
                    }
                    else if (entity.TryGetComponent(out ICollectible collectible))
                    {
                        if (collectible.GetType() == typeof(Human))
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