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
            var result = Physics2D.OverlapCircle(cell, 0.1f);

            if (result != default && result.TryGetComponent(out Inventory opponentsInventory))
            {
                if (opponentsInventory.TryPopItem(out Human human))
                {
                    inventory.Add(human);
                    return true;
                }
            }

            return false;
        }
    }
}
