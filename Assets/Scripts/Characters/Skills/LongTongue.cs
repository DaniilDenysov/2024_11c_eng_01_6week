using System;
using System.Collections.Generic;
using Cards;
using Characters;
using Characters.Skills;
using Collectibles;
using UnityEngine;

[RequireComponent(typeof(Attack)), RequireComponent(typeof(Inventory))]
public class LongTongue : Skill
{
    private Attack _attack;
    private Inventory _inventory;
    private int _attackRange = 2;
    private int _eatRange = 1;

    void Awake()
    {
        _attack = GetComponent<Attack>();
        _inventory = GetComponent<Inventory>();
    }

    public override void Activate(Action<bool> onSetUp)
    {
        base.Activate(onSetUp);
        
        List<Vector3> litPositions = _attack.GetAttackCells(_eatRange);
        litPositions.AddRange(_inventory.GetPickUpCells(_eatRange, typeof(Human), false));
        
        TileSelector.Instance.SetTilesLit(litPositions, OnCellChosen);
    }

    private void OnCellChosen(Vector3 chosenTile)
    {
        Card.AttackAndEatAtCell(chosenTile, _attack, _inventory);
        OnActivated();
    }

    public override bool IsActivatable()
    {
        return _attack.GetAttackCells(_eatRange).Count + 
            _inventory.GetPickUpCells(_eatRange, typeof(Human), false).Count > 0;
    }
}