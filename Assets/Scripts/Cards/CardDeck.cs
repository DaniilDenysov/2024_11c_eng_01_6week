using Managers;
using Selectors;
using System;
using System.Collections;
using System.Collections.Generic;
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
        
        private List<GameObject> StepCards;
        private bool isMultiCardStep;

        void Awake()
        {
            isMultiCardStep = false;
            StepCards = new List<GameObject>();
        }

        private void Update() {
            cardsContainer.SetActive(CharacterSelector.CurrentCharacter.gameObject == ownedBy);
        }

        private void StartMultiCardStepSwitch(bool arg1)
        {
            if (CharacterSelector.CurrentCharacter.gameObject == ownedBy)
            {
                if (!isMultiCardStep)
                {
                    isMultiCardStep = true;
                }
                else
                {
                    isMultiCardStep = false;
            
                    if (arg1)
                    {
                        foreach (GameObject card in StepCards)
                        {
                            Destroy(card);
                        }
                    }
                    
                    StepCards.Clear();
                }
            }
        }

        public bool GetApproval(GameObject card)
        {
            if (!isMultiCardStep)
            {
                return true;
            }
            else
            {
                if (!StepCards.Contains(card))
                {
                    StepCards.Add(card);

                    MonoBehaviour[] components = card.GetComponents<MonoBehaviour>();
                    
                    // EventManager.OnMultiStepCardUsed.Invoke(components);
                }
            
                return false;
            }
        }

        public void AddCard(String name)
        {
            
        }
    }
}
