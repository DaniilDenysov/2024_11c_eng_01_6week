using Characters;
using System.Collections;
using System.Collections.Generic;
using Ganeral;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;
using Validation;

namespace Cards
{
    public class PunchCard : Card<Attack>
    {
        [SerializeField] private TileSelector tileSelector;
        private Attack _attack;
        
        public override void OnCardActivation(Attack arg1)
        {
            CharacterMovement movement;
            
            if (!arg1.TryGetComponent(out _attack))
            {
                OnCardSetUp(false);
                return;
            }
            
            if (!arg1.TryGetComponent(out movement))
            {
                OnCardSetUp(false);
                return;
            }
            
            PathValidator pathValidator = movement.GetPathValidator();
            Vector3 characterPosition = _attack.transform.position;
            List<Vector3> directions = CoordinateManager.GetAllDirections();

            List<Vector3> litPositions = new List<Vector3>();

            foreach (Vector3 direction in directions)
            {
                if (pathValidator.CanMoveTo(characterPosition, characterPosition + direction))
                {
                    foreach (GameObject entity in CoordinateManager.GetEntities(characterPosition + direction))
                    {
                        if (entity.TryGetComponent(out Inventory _))
                        {
                            litPositions.Add(characterPosition + direction);
                        }
                    }
                }
            }

            if (litPositions.Capacity > 0)
            {
                tileSelector.SetTilesLit(litPositions);
                EventManager.OnLitTileClick += OnCellChosen;
            }
            else
            {
                OnCardSetUp(false);
            }
        }
        
        private void OnCellChosen(Vector3 chosenTile)
        {
            EventManager.OnLitTileClick -= OnCellChosen;

            OnCardSetUp(_attack.TryAttack(chosenTile));
        }
    }
}
