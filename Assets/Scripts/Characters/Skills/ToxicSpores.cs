using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cards;
using Characters;
using Characters.Skills;
using Collectibles;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterMovement)),
 RequireComponent(typeof(Attack)),
 RequireComponent(typeof(Inventory))]
public class ToxicSpores : Skill
{
    [SerializeField] private UnityEvent<Vector3, float> onSkillActivated;
    private CharacterMovement _movement;
    private Attack _attack;
    private Inventory _collector;
    private int _range = 3;
    private float _eatAndPunchDelay = 0.5f;

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
                if (usableCell.Contains(transform.position + distance * direction))
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
        float distance = 0;
        
        for (int i = 1; i < _range + 1; i++)
        {
            Vector3 previousCell = characterPosition + direction * (i - 1);
            Vector3 currentCell = characterPosition + direction * i;
            
            if (_movement.GetPathValidator().CanMoveTo(previousCell, currentCell))
            {
                distance = i;
            }
            else
            {
                break;
            }
        }
        
        onSkillActivated.Invoke(direction, distance);
        StartCoroutine(EatAndPunch(direction));
    }
    
    private IEnumerator EatAndPunch(Vector3 direction)
    {
        Vector3 characterPosition = transform.position;
        yield return new WaitForSeconds(_eatAndPunchDelay);
        bool result = false;

        for (int distance = 1; distance < _range + 1; distance++)
        {
            Vector3 currentCell = characterPosition + direction * distance;
            result = Card.AttackAndEatAtCell(currentCell, _attack, _collector) || result;
        }

        OnActivated(result);
    }

    public override bool IsActivatable()
    {
        return _attack.GetAttackCells(_range).Capacity > 0 
               || _collector.GetPickUpCells(_range, typeof(Human)).Capacity > 0;
    }
}