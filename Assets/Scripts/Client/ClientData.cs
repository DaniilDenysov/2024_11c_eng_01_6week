using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class ClientData : NetworkBehaviour
    {
        [SerializeField, SyncVar] private int cardAmount;

        [Server]
        public void SetCardAmount (int amount)
        {
            cardAmount = amount;
        }

        [Server]
        public int GetCardAmount() => cardAmount;

    }
}
