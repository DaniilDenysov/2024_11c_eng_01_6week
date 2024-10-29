using DesignPatterns.Singleton;
using System.Collections;
using System.Collections.Generic;
using UI.Inventory;
using UnityEngine;
using CustomTools;
using Collectibles;
using System.Linq;

namespace UI.Containers
{
    public class InventoryContainer : Singleton<InventoryContainer>
    {

        [SerializeField] private InventoryCell [] inventoryCells;

        [PreCompilationConstructor]
        public void Constructor ()
        {
            inventoryCells = GetComponentsInChildren<InventoryCell>();
        }

        public bool TryAdd (HumanDTO human)
        {
            foreach (var cell in inventoryCells)
            {
                if (cell.TryAdd(human))
                { 
                    return true;
                }
            }
            return false;
        }

        public bool TryRemove()
        {
            foreach (var cell in inventoryCells.Reverse())
            {
                if (cell.TryRemove())
                {
                    return true;
                }
            }
            return false;
        }

        public override InventoryContainer GetInstance()
        {
            return this;
        }
    }
}
