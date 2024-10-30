using System;
using System.Collections.Generic;
using System.Linq;
using Collectibles;
using ModestTree;
using UnityEngine;

namespace Characters
{
    public class StaticCellInventory : Inventory
    {
        private Dictionary<string, List<Vector3>> _staticPickUpCells;
        private Action<bool> _onPickedUp;

        public void Awake()
        {
            _staticPickUpCells = new Dictionary<string, List<Vector3>>();
        }

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

        public override void PickUp(Action<bool> onPickedUp)
        {
            if (IsStaticPickUpCellsEmpty())
            {
                base.PickUp(onPickedUp);
            }
            else
            {
                List<Vector3> litPositions = GetPickUpCells(0, typeof(Human));
                TileSelector.Instance.SetTilesLit(litPositions, OnCellChosen);
                _onPickedUp = onPickedUp;
            }
        }

        private void OnCellChosen(Vector3 cell)
        {
            _onPickedUp(PickUp(cell));
        }

        public override List<Vector3> GetPickUpCells(int range, Type collectibleType, bool includeStaticCell = true, bool includeCurrentCell = true)
        {
            List<Vector3> result = base.GetPickUpCells(range, collectibleType, includeStaticCell, includeCurrentCell);
            
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
    }
}