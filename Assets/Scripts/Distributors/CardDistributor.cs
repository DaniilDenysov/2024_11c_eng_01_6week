using System;
using Cards;
using Client;
using General;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Distributors
{
    public class CardDistributor : NetworkBehaviour
    {
        [SerializeField] private CardDeckDTO[] cards;
        [SerializeField, Range(0, 100)] private int cardsLimit = 6;
        [SerializeField] private CardDeckDTO[] _discardedCards;

        public static CardDistributor Instance;

        public virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            InitializeDiscardedCards();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
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
                    int diff = cardsLimit - clientData.GetCardCount() - clientData.GetHarmAmount();
                    
                    for (int i = 0; i < diff; i++)
                    {
                        int cardIndex = GetRandomAvailableCardIndex(false);
                        if (cardIndex == -1) return;
                        cards[cardIndex].amount -= 1;
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
        
        [Command(requiresAuthority = false)]
        public void CmdDiscardCard(String card)
        {
            RpcDiscardCard(card);
        }
        
        [ClientRpc]
        public void RpcDiscardCard(String card)
        {
            for (int i = 0; i < _discardedCards.Length; i++)
            {
                if (_discardedCards[i].card.gameObject.name == card)
                {
                    _discardedCards[i].amount += 1;
                    return;
                }
            }
        }

        /// <summary>
        /// Tries to get the card at the given index. If available, instantiates it.
        /// </summary>
        public bool TryGetCard(int i, out Card card)
        {
            card = Instantiate(cards[i].card);
            card.SetInitializedFrom(cards[i].card.gameObject.name);
            return true;
        }

        /// <summary>
        /// Gets a random available card index from the deck.
        /// </summary>
        /// <returns>An index of a card that has a positive amount or -1 if none are available.</returns>
        private int GetRandomAvailableCardIndex(bool isStoppable = true)
        {
            List<int> availableCards = new List<int>();

            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i].amount > 0)
                {
                    availableCards.Add(i);
                }
            }

            if (availableCards.Count == 0)
            {
                cards = _discardedCards;
                InitializeDiscardedCards();
                
                if (!isStoppable)
                {
                    return GetRandomAvailableCardIndex();
                }

                return -1;
            }

            int randomIndex = UnityEngine.Random.Range(0, availableCards.Count);
            return availableCards[randomIndex];
        }

        private void InitializeDiscardedCards()
        {
            _discardedCards = new CardDeckDTO[cards.Length];
            _discardedCards = Enumerable.Repeat(new CardDeckDTO(0), cards.Length).ToArray();

            for (int i = 0; i < _discardedCards.Length; i++)
            {
                _discardedCards[i].card = cards[i].card;
            }
        }
    }
}
