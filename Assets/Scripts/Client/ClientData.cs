using System;
using Characters;
using Managers;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Client
{
    public class ClientData : NetworkBehaviour
    {
        [SerializeField, SyncVar] private int score;
        [SerializeField, SyncVar(hook = nameof(OnScoreStateChanged))] private int cardAmount;
        [SerializeField, SyncVar] private bool myTurn;
        [SerializeField] private UnityEvent<String> onScoreChanged;

        public void OnScoreStateChanged(int oldValue, int newValue)
        {
     
        }

        [ClientRpc]
        public void RpcSetCardAmount (int amount)
        {
            cardAmount = amount;
        }

        public int GetCardAmount() => cardAmount;

        [Command(requiresAuthority = false)]
        public void CmdSetScoreAmount(int amount)
        {
            RpcSetScoreAmount(amount);
        }
        
        [ClientRpc]
        public void RpcSetScoreAmount(int amount)
        {
            score = amount;

            if (isOwned)
            {
                onScoreChanged.Invoke(amount.ToString());
            }
        }
        
        public int GetScoreAmount() => score;

        [ClientRpc]
        public void RpcSetTurn (bool turn)
        {
            if (!turn && myTurn && isOwned) EventManager.FireEvent(EventManager.OnClientEndTurn);
            myTurn = turn;
            if (myTurn && isOwned) EventManager.FireEvent(EventManager.OnClientStartTurn);
        }
    
        public bool GetTurn() => myTurn;
    }
}
