using System.Collections.Generic;
using Characters;
using Collectibles;
using Mirror;
using UnityEngine;

namespace Traps
{
    public class HorntipedeBody : NetworkBehaviour
    {
        private Attack _attack;
        private Inventory _collector;

        public void SetUp(Vector3 position, GameObject owner)
        {
            transform.position = position;
            
            if (!owner.TryGetComponent(out _attack) || !owner.TryGetComponent(out _collector))
            {
                Debug.LogError(
                    "GameObject making trail doesn't have Attack or Collection component");
            }
        }

        public bool IsCellAttackable()
        {
            return _attack.IsCellAttackable(transform.position);
        }

        public bool IsCellEatable()
        {
            return _collector.IsCellPickable(transform.position, typeof(Human));
        }

        [Command(requiresAuthority = false)]
        public void CmdRemoveFromField()
        {
            RpcRemoveFromField();
        }

        [ClientRpc]
        public void RpcRemoveFromField()
        {
            _attack = null;
            _collector = null;
            
            Destroy(gameObject);
        }
    }
}