using Client;
using DesignPatterns.Singleton;
using General;
using Managers;
using Mirror;
using UnityEngine;

namespace Distributors
{
    public class ScoreDistributor : NetworkBehaviour
    {
        public static ScoreDistributor Instance;
        [SerializeField] private int scorePerClient;
        [SerializeField] private DiceManager _diceManager;

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

        [Server]
        public void AddScoreToCurrentClient()
        {
            foreach (var client in NetworkPlayerContainer.Instance.GetItems())
            {
                if (client.TryGetComponent(out ClientData data))
                {
                    data.RpcSetScoreAmount(_diceManager.GetDiceValue());
                }
            }
        }
    }
}
