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
        [SerializeField, SyncVar] private bool isCollected;

        [Server]
        public void SetOwner(string owner)
        {
            ownedBy = owner;
        }

        private void Update()
        {
            if (isCollected && gameObject.activeInHierarchy)
            {
                gameObject.SetActive(false);
            }
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
            CmdSetCollected(true);
            return this;
        }
        
        [Command(requiresAuthority = false)]
        private void CmdSetCollected(bool isCollected)
        {
            RpcSetCollected(isCollected);
        }
    
        [ClientRpc]
        private void RpcSetCollected(bool isCollected)
        {
            this.isCollected = isCollected;
        }
    }
}
