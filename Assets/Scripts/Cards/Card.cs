using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using Selectors;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cards
{
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public abstract class Card<T> : MonoBehaviour
    {
        private CanvasGroup canvasGroup;
        private Transform container;
        private Camera camera;
        private RectTransform rectTransform;
        private Vector2 startPosition;
        private bool IsCardSettingUp;
        private CardDeck _cardDeck;

        public virtual void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            container = transform.parent;
            rectTransform = GetComponent<RectTransform>();
            camera = Camera.main;
            IsCardSettingUp = false;
            SetCardDeck();
        }

        private void Update()
        {
            Vector2 mousePos = Input.mousePosition;
            
            if (Input.GetMouseButtonUp(0) 
                && RectTransformUtility.RectangleContainsScreenPoint(rectTransform, mousePos)
                && !IsCardSettingUp)
            {
                GameObject currentPlayer = CharacterSelector.CurrentCharacter.gameObject;
                
                if (_cardDeck.GetApproval(gameObject) && currentPlayer.TryGetComponent(out T action))
                {
                    IsCardSettingUp = true;
                    canvasGroup.alpha = 0.5f;
                    OnCardActivation(action);
                }
            }
        }

        public abstract void OnCardActivation(T arg1);

        public virtual void OnCardSetUp(bool succesfully)
        {
            if (succesfully)
            {
                Destroy(gameObject);
            }

            IsCardSettingUp = false;
            canvasGroup.alpha = 1f;
        }

        public void SetCardDeck()
        {
            Transform parent = transform.parent;
            
            while (parent != null)
            {
                if (parent.TryGetComponent(out CardDeck cardDeck))
                {
                    _cardDeck = cardDeck;
                }
                
                parent = parent.transform.parent;
            }
        }
    }
}
