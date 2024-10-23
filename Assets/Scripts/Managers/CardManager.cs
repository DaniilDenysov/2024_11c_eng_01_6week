using System;
using System.Collections.Generic;
using Cards;
using Client;
using CustomTools;
using General;
using Mirror;
using UnityEngine;

namespace Managers
{
    public class CardManager : NetworkBehaviour
    {
        [SerializeField] private CardDeckDTO[] cards;
        [SerializeField, Range(0, 100)] private int cardsLimit = 6;

        public override void OnStartServer()
        {
            if (!isServer) return;
            EventManager.OnTurnStart += OnTurnStart;
            EventManager.OnTurnEnd += OnTurnEnd;
        }

        public override void OnStopServer()
        {
            if (!isServer) return;
            EventManager.OnTurnStart -= OnTurnStart;
            EventManager.OnTurnEnd -= OnTurnEnd;
        }

        private void OnTurnStart()
        {
            DistributeCardsToClients();
        }

        private void OnTurnEnd()
        {

        }

        /// <summary>
        /// Distributes cards across all clients if limit isn't reached
        /// </summary>
        [Server]
        public void DistributeCardsToClients()
        {
            foreach (NetworkPlayer player in NetworkPlayerContainer.Instance.GetItems())
            {
                if (player.connectionToClient != null && player.TryGetComponent(out ClientData clientData))
                {
                    int diff = cardsLimit - clientData.GetCardAmount();
                    for (int i = 0; i < diff; i++)
                    {
                        int cardIndex = GetRandomAvailableCardIndex();
                        if (cardIndex == -1) return;
                        cards[cardIndex].Amount-=1;
                        AddCard(player.netIdentity.connectionToClient, cardIndex);
                    }
                }
            }
        }

        /// <summary>
        /// Tries to add the card to the client via TargetRpc.
        /// </summary>
        [TargetRpc]
        public void AddCard(NetworkConnection conn, int cardIndex)
        {
            if (TryGetCard(cardIndex, out Card card))
            {
                ClientDeck.Instance.Add(card);
            }
        }

        /// <summary>
        /// Tries to get the card at the given index. If available, instantiates it.
        /// </summary>
        public bool TryGetCard(int i, out Card card)
        {
            card = Instantiate(cards[i].Card);
            return true;
        }

        /// <summary>
        /// Gets a random available card index from the deck.
        /// </summary>
        /// <returns>An index of a card that has a positive amount or -1 if none are available.</returns>
        private int GetRandomAvailableCardIndex()
        {
            List<int> availableCards = new List<int>();

            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i].Amount > 0)
                {
                    availableCards.Add(i);
                }
            }

            if (availableCards.Count == 0)
            {
                return -1;
            }

            int randomIndex = UnityEngine.Random.Range(0, availableCards.Count);
            return availableCards[randomIndex];
        }
    }
}
