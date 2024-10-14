using Managers;
using Selectors;
using System;
using System.Collections;
using System.Collections.Generic;
using Characters;
using General;
using ModestTree;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Cards
{
    public class CardDeck : MonoBehaviour
    {
        [SerializeField] private GameObject ownedBy;
        [SerializeField] private GameObject cardsContainer;
        [SerializeField] private CardManager cardManager;
        
        private const int cardNumber = 4;
        
        private bool _isMultiCardStep;
        private CharacterStateManager _stateManager;

        void Awake()
        {
            _isMultiCardStep = false;
            
            if (!ownedBy.TryGetComponent<>(out _stateManager))
            {
                Debug.LogError("Failed to find state manager on card deck owner");
            }
        }

        public CharacterStateManager GetStateManager()
        {
            return _stateManager;
        }

        private void Start()
        {
            EventManager.OnTurnEnd += OnTurnEnd;
            
            for (int i = 0; i < cardNumber; i++)
            {
                AddCard();
            }
        }

        private void Update() {
            cardsContainer.SetActive(CharacterSelector.CurrentCharacter.gameObject == ownedBy);
        }

        private int GetCardNumber()
        {
            return cardsContainer.transform.childCount;
        }

        private void AddCard()
        {
            CardPoolable newCard = cardManager.GetRandomCard();
            
            if (newCard.TryGetComponent(out Card card))
            {
                card.SetUp(ownedBy, _stateManager);
            }
            else
            {
                Debug.LogError("Failed to find Card Component on card: " + newCard.name);
            }
            
            newCard.transform.SetParent(cardsContainer.transform);
            newCard.gameObject.SetActive(true);
        }

        private void OnTurnEnd()
        {
            for (int i = GetCardNumber(); i < cardNumber; i++)
            {
                AddCard();
            }
        }
    }
}
