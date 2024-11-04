using System;
using Client;
using General;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using Characters;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Distributors
{
    public class CharacterTurnDistributor : NetworkBehaviour
    {
        private Queue<NetworkPlayer> order;
        public static CharacterTurnDistributor Instance;
        [SerializeField] private UnityEvent onLocalTurnStart;
        [SerializeField] private UnityEvent onLocalTurnEnd;

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
            if (player.connectionToClient != null)
            {
                OnLocalTurn(player.connectionToClient, true);

                if (player.TryGetComponent(out ClientData data))
                {
                    data.RpcSetTurn(true);
                }
            }
        }

        [ClientRpc]
        private void OnLocalTurnStart()
        {
            
        }

        [Server]
        public void OnTurnEnd()
        {
            var player = order.Dequeue();
            OnLocalTurn(player.connectionToClient, false);

            if (player.TryGetComponent(out ClientData data))
            {
                data.RpcSetTurn(false);
                order.Enqueue(player);
            }
        }
        
        [TargetRpc]
        private void OnLocalTurn(NetworkConnection target, bool isStart)
        {
            if (isStart)
            {
                onLocalTurnStart.Invoke();
            }
            else
            {
                onLocalTurnEnd.Invoke();
            }
        }
    }
}
