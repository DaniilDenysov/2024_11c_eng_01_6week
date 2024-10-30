using Collectibles;
using CustomTools;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Collectibles
{
    public class Human : ICollectible
    {
        [SerializeField, SyncVar] private string ownedBy;
        [SerializeField, Range(2, 6), SyncVar] private int currentPoints;

        [Server]
        public void SetOwner(string owner)
        {
            ownedBy = owner;
        }

        public HumanDTO GetData()
        {
            return new HumanDTO()
            {
                CharacterGUID = ownedBy,
                Amount = currentPoints
            };
        }

        public override object Collect()
        {
            CmdCollect();
            return this;
        }
        
        [Command(requiresAuthority = false)]
        private void CmdCollect()
        {
            RpcSetCollected();
        }
    
        [ClientRpc]
        private void RpcSetCollected()
        {
            Destroy(gameObject);
        }
    }
}
