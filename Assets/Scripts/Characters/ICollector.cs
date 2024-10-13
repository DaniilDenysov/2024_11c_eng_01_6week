using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Characters;
using Ganeral;
using ModestTree;
using UnityEngine;
using Validation;

namespace Collectibles
{
    [RequireComponent(typeof(CharacterMovement))]
    public abstract class ICollector : MonoBehaviour
    {
        private Dictionary<string, List<Vector3>> _staticPickUpCells;
        private CharacterMovement _movement;
        
        public void Awake()
        {
            _movement = GetComponent<CharacterMovement>();
            _staticPickUpCells = new Dictionary<string, List<Vector3>>();
        }
        
        public abstract void PickUp(Action<bool> onPickedUp);
        public abstract bool PickUp(Vector3 cell);
        
        public void AddStaticPickUpCells(List<Vector3> cells, string groupName)
        {
            if (_staticPickUpCells.TryGetValue(groupName, out List<Vector3> groupCells))
            {
                groupCells.AddRange(cells);
                groupCells = groupCells.Distinct().ToList();
                
                _staticPickUpCells[groupName] = groupCells;
            }
            else
            {
                _staticPickUpCells.Add(groupName, cells);
            }
        }
        
        public void RemoveStaticPickUpCells(string groupName)
        {
            _staticPickUpCells.Remove(groupName);
        }

        public bool IsStaticPickUpCellsEmpty()
        {
            return _staticPickUpCells.IsEmpty();
        }

        public List<Vector3> GetPickUpCells(int range, Type collectibleType, bool includeStaticCell = true, 
            bool includeCurrentCell = true)
        {
            PathValidator pathValidator = _movement.GetPathValidator();
            Vector3 characterPosition = transform.position;
            List<Vector3> directions = 
                includeCurrentCell ? CoordinateManager.GetAttackDirections() : CoordinateManager.GetAllDirections();

            List<Vector3> result = new List<Vector3>();

            foreach (Vector3 direction in directions)
            {
                for (int distance = 0; distance < range; distance++)
                {
                    Vector3 currentCell = characterPosition + direction * (distance + 1);

                    if (!pathValidator.CanMoveTo(characterPosition, currentCell))
                    {
                        break;
                    }

                    if (IsCellPickable(currentCell, collectibleType))
                    {
                        result.Add(currentCell);
                    }
                }
            }
            
            if (includeStaticCell)
            {
                foreach (var group in _staticPickUpCells)
                {
                    foreach (var cell in group.Value)
                    {
                        if (!result.Contains(cell))
                        {
                            if (IsCellPickable(cell, collectibleType))
                            {
                                result.Add(cell);
                            }
                        }
                    }
                }
            }

            return result;
        }
        
        public bool IsCellPickable(Vector3 cell, Type collectibleType)
        {
            foreach (GameObject entity in CoordinateManager.GetEntities(cell))
            {
                if (IsPickable(entity, collectibleType))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsPickable(GameObject entity, Type collectibleType)
        {
            return entity.TryGetComponent(out ICollectible collectible)
                   && collectible.GetType() == collectibleType;
        }
    }
}
