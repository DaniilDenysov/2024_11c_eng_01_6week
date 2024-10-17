using System;
using System.Collections.Generic;
using System.Linq;
using Cards;
using Characters;
using Characters.Skills;
using Collectibles;
using Ganeral;
using UnityEngine;

[RequireComponent(typeof(CharacterMovement)),
 RequireComponent(typeof(Attack)),
 RequireComponent(typeof(ICollector))]
public class ToxicSpores : Skill
{
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

    public override void Activate(Action<bool> onSetUp)
    {
        base.Activate(onSetUp);
        
        List<Vector3> litPositions = _attack.GetAttackCells(_range, false, false);
        litPositions.AddRange(_collector.GetPickUpCells(_range, typeof(Human), false, false));

        litPositions = litPositions.Distinct().ToList();

        TileSelector.Instance.SetTilesLit(litPositions, OnDirectionChosen);
    }

    private void OnDirectionChosen(Vector3 chosenTile)
    {
        Vector3 characterPosition = transform.position;
        Vector3 direction = chosenTile - characterPosition;
        bool result = false;

        for (int i = 1; i < _range; i++)
        {
            Vector3 currentCell = characterPosition + direction * i;
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