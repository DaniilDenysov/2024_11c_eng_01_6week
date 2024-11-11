using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UI;
using UnityEngine.Events;

namespace General
{
    public class NetworkPlayerContainer : LabelContainer<NetworkPlayer, NetworkPlayerContainer>
    {
        public int CalculateToatlScoreForPlayer(NetworkPlayer player)
        {
            int score = 0;
            if (player.gameObject.TryGetComponent(out Inventory inventory))
            {
                foreach (var human in inventory.GetHumans())
                {
                    score += human.Amount;
                }
            }
            return score;
        }

        public override NetworkPlayerContainer GetInstance()
        {
            return this;
        }

        public NetworkPlayer GetOwner(MonoBehaviour behaviour)
        {
            foreach (var item in items)
            {
                if (item.gameObject == behaviour.gameObject)
                {
                    return item;
                }
            }

            return null;
        }
    }
}