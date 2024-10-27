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
        [SerializeField, SyncVar] private bool myTurn;

        public void OnScoreStateChanged(int oldValue, int newValue)
        {
     
        }

        [ClientRpc]
        public void RpcSetCardAmount (int amount)
        {
            cardAmount = amount;
        }

        public int GetCardAmount() => cardAmount;


        [Command]
        public void CmdSetScoreAmount(int amount)
        {
            score = amount;
        }
        
        [ClientRpc]
        public void RpcSetScoreAmount(int amount)
        {
            score = amount;
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
