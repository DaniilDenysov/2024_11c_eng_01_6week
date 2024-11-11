using Distributors;
using Mirror;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    public class TurnManager : NetworkBehaviour
    {

        [SerializeField,Range(0,600)] private float turnTimeLimit = 180f, turnDelay = 30f;
        private Coroutine currentTurn;
        [SerializeField] private UnityEvent OnNextTurnStart;

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

        [ClientRpc]
        public void RPCOnTurnEnd()
        {
            OnNextTurnStart.Invoke();
            TileSelector.Instance.SetTilesUnlit();
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
            GlobalMessageContainer.Instance.DisplayMessage($"Turn of {CharacterTurnDistributor.Instance.GetCurrentPlayer().GetCharacterData().ChracterName}", "New turn!");
            yield return new WaitForSeconds(turnDelay);
            ScoreDistributor.Instance.AddScoreToCurrentClient();
            CardDistributor.Instance.DistributeCardsToClients();
            CharacterTurnDistributor.Instance.OnTurnStart();
            SyncedClock.Instance.StartTimer(turnTimeLimit);
            OnTurnStart();
            yield return new WaitForSeconds(turnTimeLimit);
            EndTurn();
            currentTurn = null;
            StartNextTurn();
        }

        private void EndTurn()
        {
            RPCOnTurnEnd();
            
            CharacterTurnDistributor.Instance.OnTurnEnd();
            SyncedClock.Instance.DeleteTimer();
            OnTurnEnd();
        }
    }
}
