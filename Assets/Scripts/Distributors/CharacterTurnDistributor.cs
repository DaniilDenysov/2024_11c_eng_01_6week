using Client;
using General;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using Characters;
using UnityEngine;

namespace Distributors
{
    public class CharacterTurnDistributor : NetworkBehaviour
    {
        private Queue<NetworkPlayer> order;

        public static CharacterTurnDistributor Instance;

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

        [Server]
        public void OnTurnStart()
        {
           var player = order.Peek();
           if (player.TryGetComponent(out ClientData data))
           {
                Debug.Log(player.name + " turn");
                data.SetTurn(true);
           }
        }

        [Server]
        public void OnTurnEnd()
        {
            var player = order.Dequeue();
            if (player.TryGetComponent(out ClientData data))
            {
                data.SetTurn(false);
                order.Enqueue(player);
            }
        }
    }
}
