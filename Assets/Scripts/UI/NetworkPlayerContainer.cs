using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;

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
                    score += human.Amount * (human.CharacterGUID == player.GetCharacterGUID() ? 1 : 2);
                }
            }
            return score;
        }

        public override NetworkPlayerContainer GetInstance()
        {
            return this;
        }
    }
}