using System.Collections;
using System.Collections.Generic;
using Characters;
using Characters.Skills;
using Ganeral;
using UnityEngine;
using Validation;

[RequireComponent(typeof(CharacterMovement))]
public class Extendability : Skill
{
    [SerializeField] private TileSelector selector;
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

        foreach (Vector3 direction in directions) {
            if (pathValidator.CanMoveTo(characterPosition, characterPosition + direction) && 
                pathValidator.CanMoveTo(characterPosition, characterPosition + direction * 2) && 
                !pathValidator.CanMoveTo(characterPosition, characterPosition + direction * 3)) {
                
                litPositions.Add(characterPosition + direction);
            }
        }

        
    }
}
