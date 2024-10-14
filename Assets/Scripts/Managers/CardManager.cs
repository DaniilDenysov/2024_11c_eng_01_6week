using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Cards;
using General;
using Selectors;
using UnityEngine;
using UnityEngine.Serialization;

namespace Managers
{
    public class CardManager : Manager
    {
        [SerializeField] private SerializedDictionary<CardPoolable, int> cards = new();
        private List<string> _deck;
        private int _nextPoolable;
        private Dictionary<string, List<CardPoolable>> _pool;
        private List<string> _discardedCards;

        private void Awake()
        {
            _pool = new Dictionary<string, List<CardPoolable>>();
            _nextPoolable = 0;
            InitializeDeck();
        }

        public CardPoolable GetRandomCard()
        {
            if (_nextPoolable < _deck.Count)
            {
                _nextPoolable++;
                return Get(_deck[_nextPoolable - 1]);
            }
            else
            {
                _deck = _discardedCards;
                _deck.Shuffle();
                _discardedCards = new List<string>();
                _nextPoolable = 0;
                return Get(_deck[_nextPoolable]);
            }
        }

        private CardPoolable Get(string cardName)
        {
            if (_pool.TryGetValue(cardName, out List<CardPoolable> objectList))
            {
                CardPoolable instance = objectList[0];
                objectList.RemoveAt(0);

                return instance;
            }
            else
            {
                CardPoolable card = GetCardByName(cardName);
                
                if (card != null)
                {
                    CardPoolable instance = Instantiate(card);
                    instance.SetUp(this, cardName);
                    return instance;
                }
                
                Debug.LogError("Failed to find card poolable: " + cardName);
                return null;
            }
        }
        
        public void Put(CardPoolable cardObject)
        {
            if (_pool.TryGetValue(cardObject.GetPoolableName(), out List<CardPoolable> cardList))
            {
                cardList.Add(cardObject);
            }
            else
            {
                _pool.Add(cardObject.GetPoolableName(), new List<CardPoolable> { cardObject });
            }
            
            _discardedCards.Add(cardObject.GetPoolableName());
            
            cardObject.gameObject.SetActive(false);
        }

        public CardPoolable GetCardByName(string cardName)
        {
            foreach (CardPoolable card in cards.Keys)
            {
                if (card.gameObject.name == cardName)
                {
                    return card;
                }
            }

            return null;
        }

        public override void InstallBindings()
        {
            Container.Bind<CardManager>().FromInstance(this).AsSingle();
        }

        private void InitializeDeck()
        {
            _deck = new List<string>();
            
            foreach (var card in cards)
            {
                string cardName = card.Key.gameObject.name;
                
                for (int i = 0; i < card.Value; i++)
                {
                    _deck.Add(cardName);
                }
            }
            
            _deck.Shuffle();
        }
    }
}
