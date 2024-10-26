using Characters;
using Managers;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class ClientData : NetworkBehaviour
    {
        [SerializeField, SyncVar] private int score;
        [SerializeField, SyncVar(hook = nameof(OnScoreStateChanged))] private int cardAmount;
        [SerializeField, SyncVar(hook = nameof(OnMyTurnChanged))] private bool myTurn;

        public void OnMyTurnChanged (bool oldValue, bool newValue)
        {
            if (newValue && isOwned) EventManager.FireEvent(EventManager.OnClientStartTurn);
            if (!newValue && oldValue && isOwned) EventManager.FireEvent(EventManager.OnClientEndTurn);
        }

        public void OnScoreStateChanged(int oldValue, int newValue)
        {
     
        }

        [Server]
        public void SetCardAmount (int amount)
        {
            cardAmount = amount;
        }

        public int GetCardAmount() => cardAmount;


        [Server]
        public void SetScoreAmount(int amount)
        {
            score = amount;
        }


        public int GetScoreAmount() => score;


        [Server]
        public void SetTurn (bool turn)
        {
            myTurn = turn;
        }


        public bool GetTurn() => myTurn;

    }
}
