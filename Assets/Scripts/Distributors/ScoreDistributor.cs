using Client;
using General;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Distributors
{
    public class ScoreDistributor : Distributor
    {
        [SerializeField] private int scorePerClient;


        public override void OnTurnStart()
        {
            foreach (var client in NetworkPlayerContainer.Instance.GetItems())
            {
                if (client.TryGetComponent(out ClientData data))
                {
                    data.SetScoreAmount(scorePerClient);
                }
            }
        }
    }
}
