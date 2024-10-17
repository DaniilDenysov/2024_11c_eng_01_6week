using System;
using System.Collections.Generic;
using System.Linq;
using Cards;
using Characters;
using Characters.Skills;
using Ganeral;
using Managers;
using UnityEngine;
using Validation;

[RequireComponent(typeof(CharacterMovement)), RequireComponent(typeof(Attack))]
public class LongTongue : Skill
{
    [SerializeField] private TileSelector directionSelector;
    private CharacterMovement movement;
    private Attack attack;

    void Awake()
    {
        movement = GetComponent<CharacterMovement>();
        attack = GetComponent<Attack>();
    }

    public override void Activate()
    {
        EventManager.OnMultiStepSwitch(true);
        EventManager.OnMultiStepCardUsed += OnCardUsed;
    }

    private void OnCardUsed(MonoBehaviour[] cardComponents)
    {
        EventManager.OnMultiStepCardUsed -= OnCardUsed;

        bool isPunchCard = false;

        foreach (MonoBehaviour mono in cardComponents)
        {
            if (mono.GetType() == typeof(PunchCard))
            {
                isPunchCard = true;
            }
        }
        
        if (isPunchCard)
        {
            PathValidator pathValidator = movement.GetPathValidator();
            Vector3 characterPosition = transform.position;
            List<Vector3> directions = CoordinateManager.GetAllDirections();

            List<Vector3> litPositions = new List<Vector3>();

            foreach (Vector3 direction in directions)
            {
                if (pathValidator.CanMoveTo(characterPosition, characterPosition + direction * 2))
                {
                    foreach (GameObject entity in CoordinateManager.GetEntities(characterPosition + direction * 2))
                    {
                        if (entity.TryGetComponent(out Inventory _))
                        {
                            litPositions.Add(characterPosition + direction * 2);
                        }
                    }
                }
            }

            directionSelector.SetTilesLit(litPositions);
            EventManager.OnLitTileClick += OnCellChosen;
        } else {
            EventManager.OnMultiStepSwitch(false);
            EventManager.OnSkillSetUp(false);
        }
    }

    private void OnCellChosen(Vector3 chosenTile)
    {
        EventManager.OnLitTileClick -= OnCellChosen;
        attack.TryAttack(chosenTile);
        OnActivated();
    }

    public override bool IsActivatable()
    {
        PathValidator pathValidator = movement.GetPathValidator();
        Vector3 characterPosition = transform.position;
        List<Vector3> directions = CoordinateManager.GetAllDirections();

        foreach (Vector3 direction in directions)
        {
            if (pathValidator.CanMoveTo(characterPosition, characterPosition + direction * 2))
            {
                foreach (GameObject entity in CoordinateManager.GetEntities(characterPosition + direction * 2))
                {
                    if (entity.TryGetComponent(out Inventory _))
                    {
                        return true;
                    }
                }
            }
        }
        
        return false;
    }
}