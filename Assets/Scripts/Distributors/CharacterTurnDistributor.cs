using Client;
using General;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using Characters;
using UnityEngine;
using DesignPatterns.Singleton;

namespace Distributors
{
    public class CharacterTurnDistributor : NetworkSingleton<CharacterTurnDistributor>
    {
        private Queue<NetworkPlayer> order;

        public override CharacterTurnDistributor GetInstance()
        {
            return this;
        }

        public override void OnStartServer()
        {
            if (isServer)
            {
                order = new Queue<NetworkPlayer>(NetworkPlayerContainer.Instance.GetItems());
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
            if (player.TryGetComponent(out ClientData data))
            {
                data.RpcSetTurn(false);
                order.Enqueue(player);
            }
        }
    }
}
