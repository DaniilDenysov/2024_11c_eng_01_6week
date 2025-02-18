using System;
using System.Collections.Generic;
using Characters;
using Characters.Skills;
using UnityEngine;

[RequireComponent(typeof(CharacterMovement)), RequireComponent(typeof(Attack))]
public class LongTongue : Skill
{
    private CharacterMovement _movement;
    private Attack _attack;
    private int _range = 2;

    void Awake()
    {
        _movement = GetComponent<CharacterMovement>();
        _attack = GetComponent<Attack>();
    }

    public override void Activate(Action<bool> onSetUp)
    {
        base.Activate(onSetUp);
        
        List<Vector3> litPositions = _attack.GetAttackCells(_range);
        
        TileSelector.Instance.SetTilesLit(litPositions, OnCellChosen);
    }

    private void OnCellChosen(Vector3 chosenTile)
    {
        _attack.TryAttack(chosenTile);
        OnActivated();
    }

    public override bool IsActivatable()
    {
        return _attack.GetAttackCells(_range).Capacity > 0;
    }
}