using Managers;
using Selectors;
using System;
using System.Collections;
using System.Collections.Generic;
using General;
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
        
        private List<GameObject> _stepCards;
        private bool _isMultiCardStep;

        void Awake()
        {
            _isMultiCardStep = false;
            _stepCards = new List<GameObject>();

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

        // private void StartMultiCardStepSwitch(bool arg1)
        // {
        //     if (CharacterSelector.CurrentCharacter.gameObject == ownedBy)
        //     {
        //         if (!isMultiCardStep)
        //         {
        //             isMultiCardStep = true;
        //         }
        //         else
        //         {
        //             isMultiCardStep = false;
        //     
        //             if (arg1)
        //             {
        //                 foreach (GameObject card in StepCards)
        //                 {
        //                     Destroy(card);
        //                 }
        //             }
        //             
        //             StepCards.Clear();
        //         }
        //     }
        // }
        
        public bool GetApproval(GameObject card)
        {
            if (!_isMultiCardStep)
            {
                return true;
            }
            else
            {
                if (!_stepCards.Contains(card))
                {
                    _stepCards.Add(card);
        
                    MonoBehaviour[] components = card.GetComponents<MonoBehaviour>();
                    
                    // EventManager.OnMultiStepCardUsed.Invoke(components);
                }
            
                return false;
            }
        }
    }
}
