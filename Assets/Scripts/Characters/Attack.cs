using System.Collections;
using System.Collections.Generic;
using Ganeral;
using UnityEngine;

namespace Characters
{
    [RequireComponent(typeof(Inventory))]
    public class Attack : MonoBehaviour
    {
        private Inventory inventory;

        private void Awake()
        {
            inventory = GetComponent<Inventory>();
        }

        public bool TryAttack(Vector3 cell)
        {
            bool result = false;
            
            foreach (GameObject entity in CoordinateManager.GetEntities(cell))
            {
                if (entity.TryGetComponent(out Inventory opponentsInventory))
                {
                    if (opponentsInventory.TryPopItem(out Human human))
                    {
                        inventory.Add(human);
                        result = true;
                    }
                }
            }
            
            return result;
        }
    }
}
