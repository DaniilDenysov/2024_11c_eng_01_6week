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

        [Server]
        private void Start()
        {
            StartNextTurn();
        }

        [ClientRpc]
        private void OnTurnStart()
        {
            LocalPlayerLogContainer.Instance.AddLogMessage("Turn started");
            EventManager.FireEvent(EventManager.OnTurnStart);
        }

        [ClientRpc]
        private void OnTurnEnd()
        {
            LocalPlayerLogContainer.Instance.AddLogMessage("Turn ended");
            EventManager.FireEvent(EventManager.OnTurnEnd);
        }

        [Server]
        public void StartNextTurn ()
        {
            if (currentTurn != null) StopCoroutine(currentTurn);
            currentTurn = StartCoroutine(StartTurn());
        }

        private IEnumerator StartTurn ()
        {
            OnTurnStart();
            yield return new WaitForSeconds(turnTimeLimit);
            OnTurnEnd();
            yield return new WaitForSeconds(turnDelay);
        }

    }
}
