using Client;
using General;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Distributors
{
    public class CharacterTurnDistributor : Distributor
    {
        private Queue<NetworkPlayer> order;

        public override void OnStartServer()
        {
            if (isServer)
            {
                order = new Queue<NetworkPlayer>(NetworkPlayerContainer.Instance.GetItems());
            }
        }

        public override void OnTurnStart()
        {

           var player = order.Peek();
           if (player.TryGetComponent(out ClientData data))
           {
                data.SetTurn(true);
           }
        }

        public override void OnTurnEnd()
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
