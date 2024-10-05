using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Characters
{
    [RequireComponent(typeof(Inventory))]
    public class Attack : MonoBehaviour
    {
        [SerializeField] private List<Vector3> attackDirections = new List<Vector3>();
        private Inventory inventory;

        private void Awake()
        {
            inventory = GetComponent<Inventory>();
        }

        public bool TryAttack ()
        {
            Vector3 origin = transform.position;
            foreach (var attackDirection in attackDirections)
            {
                var result = Physics2D.OverlapCircle(origin + attackDirection, 0.1f);
                Debug.DrawRay(transform.position,attackDirection,Color.red,10f);
                if (result != default && result.TryGetComponent(out Inventory opponentsInventory))
                {
                    //Add zone handling
                    if (opponentsInventory.TryPopItem(out Human human))
                    {
                        inventory.Add(human);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
