using Collectibles;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Distributors;
using UnityEngine;
using Validation;

namespace Characters
{
    [RequireComponent(typeof(Inventory)), RequireComponent(typeof(CharacterMovement))]
    public class Attack : MonoBehaviour
    {
        private Inventory _inventory;
        private CharacterMovement _movement;
        private Dictionary<string, List<Vector3>> _staticAttackCells;

        private void Awake()
        {
            _inventory = GetComponent<Inventory>();
            _movement = GetComponent<CharacterMovement>();
            _staticAttackCells = new Dictionary<string, List<Vector3>>();
        }

        public void AddStaticAttackCells(List<Vector3> cells, string groupName)
        {
            if (_staticAttackCells.TryGetValue(groupName, out List<Vector3> groupCells))
            {
                groupCells.AddRange(cells);
                groupCells = groupCells.Distinct().ToList();

                _staticAttackCells[groupName] = groupCells;
            }
            else
            {
                _staticAttackCells.Add(groupName, cells);
            }
        }
        
        public void RemoveStaticAttackCells(string groupName)
        {
            _staticAttackCells.Remove(groupName);
        }

        public List<Vector3> GetAttackCells(int range, bool includeStaticCell = true, bool includeCurrentCell = true)
        {
            PathValidator pathValidator = _movement.GetPathValidator();
            Vector3 characterPosition = transform.position;
            List<Vector3> directions = 
                includeCurrentCell ? CharacterMovement.GetAttackDirections() : CharacterMovement.GetAllDirections();

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
                
                    if (IsCellAttackable(currentCell))
                    {
                        result.Add(currentCell);
                    }
                }
            }

            if (includeStaticCell)
            {
                foreach (var group in _staticAttackCells)
                {
                    foreach (var cell in group.Value)
                    {
                        if (!result.Contains(cell))
                        {
                            if (IsCellAttackable(cell))
                            {
                                result.Add(cell);
                            }
                        }
                    }
                }
            }

            return result;
        }

        public bool IsCellAttackable(Vector3 cell)
        {
            foreach (GameObject entity in CharacterMovement.GetEntities(cell))
            {
                if (IsEntityAttackable(entity))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsEntityAttackable(GameObject entity)
        {
            return entity.TryGetComponent(out Inventory _) && entity != gameObject;
        }

        public bool TryAttack(Vector3 cell)
        {
            bool result = false;
            
            foreach (GameObject entity in CharacterMovement.GetEntities(cell))
            {
                if (entity.TryGetComponent(out NetworkPlayer player))
                {
                    if (CardDistributor.Instance.GetHarmCount(player) > 0)
                    {
                        CardDistributor.Instance.CmdDecreaseHarmCount(player);
                        result = true;
                    } else if (entity.TryGetComponent(out Inventory opponentsInventory))
                    {
                        if (opponentsInventory.TryPopItem(out Human human))
                        {
                            _inventory.CmdAddCollectibleToInventory(human);
                            result = true;
                        }
                    }
                }
            }
            
            return result;
        }
    }
}
