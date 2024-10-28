using System;
using System.Collections.Generic;
using System.Linq;
using Cards;
using Characters;
using Characters.Skills;
using Collectibles;
using UnityEngine;

[RequireComponent(typeof(CharacterMovement)),
 RequireComponent(typeof(Attack)),
 RequireComponent(typeof(Inventory))]
public class ToxicSpores : Skill
{
    private CharacterMovement _movement;
    private Attack _attack;
    private Inventory _collector;
    private int _range = 3;

    void Awake()
    {
        _movement = GetComponent<CharacterMovement>();
        _collector = GetComponent<Inventory>();
        _attack = GetComponent<Attack>();
    }

    public override void Activate(Action<bool> onSetUp)
    {
        base.Activate(onSetUp);
        List<Vector3> excludedDirections = new List<Vector3>();
        List<Vector3> usableCell = _attack.GetAttackCells(_range, false, false);
        usableCell.AddRange(_collector.GetPickUpCells(_range, typeof(Human), false, false));

        foreach (Vector3 direction in CharacterMovement.GetAllDirections())
        {
            bool isUsableDirection = false;
                
            for (int distance = 1; distance < _range + 1; distance++)
            {
                if (usableCell.Contains(transform.position + _range * direction))
                {
                    isUsableDirection = true;
                    break;
                }
            }

            if (!isUsableDirection)
            {
                excludedDirections.Add(direction);
            }
        }

        TileSelector.Instance.SetDirectionsTilesLit(transform.position, OnDirectionChosen, excludedDirections);
    }

    private void OnDirectionChosen(Vector3 chosenTile)
    {
        Vector3 characterPosition = transform.position;
        Vector3 direction = chosenTile - characterPosition;
        bool result = false;

        for (int distance = 1; distance < _range + 1; distance++)
        {
            Vector3 currentCell = characterPosition + direction * distance;
            result = result || Card.AttackAndEatAtCell(currentCell, _attack, _collector);
        }

        OnActivated(result);
    }

    public override bool IsActivatable()
    {
        return _attack.GetAttackCells(_range).Capacity > 0 
               || _collector.GetPickUpCells(_range, typeof(Human)).Capacity > 0;
    }
}