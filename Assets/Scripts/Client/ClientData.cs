using System;
using Characters;
using Managers;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Client
{
    public class ClientData : NetworkBehaviour
    {
        [SerializeField, SyncVar] private int score;
        [SerializeField, SyncVar] private int harmAmount;
        [SerializeField, SyncVar] private bool myTurn;
        [SerializeField, SyncVar] private int _cardCount;
        [SerializeField] private UnityEvent<String> onScoreChanged;

        [Command(requiresAuthority = false)]
        public void CmdChangeHarmAmount(bool isAdded)
        {
            RpcChangeHarmAmount(isAdded);
        }
        
        private void Start()
        {
            if (isOwned)
            {
                EventManager.OnCardCountChange += CmdSetCardCount;
            }
        }
        
        [ClientRpc]
        private void RpcChangeHarmAmount(bool isAdded)
        {
            if (isAdded)
            {
                harmAmount++;
            }
            else
            {
                harmAmount--;
            }
        }

        public int GetHarmAmount() => harmAmount;

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

        [Command(requiresAuthority = false)]
        public void CmdSetCardCount(int newCount)
        {
            RpcSetCardCount(newCount);
        }

        [ClientRpc]
        private void RpcSetCardCount(int newCount)
        {
            _cardCount = newCount;
        }
        
        public int GetCardCount()
        {
            return _cardCount;
        }

        private void OnDestroy()
        {
            EventManager.OnCardCountChange -= CmdSetCardCount;
        }
    }
}
