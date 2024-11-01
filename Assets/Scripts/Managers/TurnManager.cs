using Distributors;
using Mirror;
using System;
using System.Collections;
using UnityEngine;

namespace Managers
{
    public class TurnManager : NetworkBehaviour
    {

        [SerializeField,Range(0,600)] private float turnTimeLimit = 180f, turnDelay = 30f;
        private Coroutine currentTurn;

        private void Start()
        {
            if (!isServer) return;
            StartNextTurn();
        }

        [Command(requiresAuthority = false)]
        public void SetTurnChanged()
        {
            StartNextTurn();
        }

        private void OnTurnStart()
        {
            Debug.Log("Turn started");
            LocalPlayerLogContainer.Instance.AddLogMessage("Turn started");
            EventManager.FireEvent(EventManager.OnTurnStart);
        }

        [ClientRpc]
        private void OnTurnEnd()
        {
            Debug.Log("Turn ended");
            LocalPlayerLogContainer.Instance.AddLogMessage("Turn ended");
            EventManager.FireEvent(EventManager.OnTurnEnd);
        }

        [Server]
        public void StartNextTurn()
        {
            if (currentTurn != null)
            {
                StopCoroutine(currentTurn);
                EndTurn();
            }
            
            currentTurn = StartCoroutine(StartTurn());
        }

        private IEnumerator StartTurn()
        {
            CharacterTurnDistributor.Instance.OnTurnStart();
            ScoreDistributor.Instance.AddScoreToCurrentClient();
            CardDistributor.Instance.DistributeCardsToClients();
            SyncedClock.Instance.StartTimer(turnTimeLimit);
            OnTurnStart();
            yield return new WaitForSeconds(turnTimeLimit);
            EndTurn();
            currentTurn = null;
            StartNextTurn();
        }

        private void EndTurn()
        {
            CharacterTurnDistributor.Instance.OnTurnEnd();
            SyncedClock.Instance.DeleteTimer();
            OnTurnEnd();
        }
    }
}
