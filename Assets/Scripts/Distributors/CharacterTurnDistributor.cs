using System;
using Client;
using General;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using Characters;
using UnityEngine;
using UnityEngine.Events;

namespace Distributors
{
    public class CharacterTurnDistributor : NetworkBehaviour
    {
        private Queue<NetworkPlayer> order;
        public static CharacterTurnDistributor Instance;
        [SerializeField] private UnityEvent onTurnEnd;

        public virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public override void OnStartServer()
        {
            if (isServer)
            {
                order = new Queue<NetworkPlayer>(NetworkPlayerContainer.Instance.GetItems());
                Debug.Log(order.Count);
            }
        }

        public NetworkPlayer GetCurrentPlayer() => order.Peek();

        [Server]
        public void OnTurnStart()
        {
            var player = order.Peek();
            if (player.TryGetComponent(out ClientData data))
            {
                data.RpcSetTurn(true);
            }
        }

        [Server]
        public void OnTurnEnd()
        {
            var player = order.Dequeue();
            
            onTurnEnd.Invoke();
            
            if (player.TryGetComponent(out ClientData data))
            {
                data.RpcSetTurn(false);
                order.Enqueue(player);
            }
        }
    }
}
