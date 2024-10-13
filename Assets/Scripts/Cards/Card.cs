using System;
using System.Collections;
using System.Collections.Generic;
using General;
using Managers;
using Selectors;
using Shooting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cards
{
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public abstract class Card<T> : CardPoolable
    {
        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;
        private Vector2 _startPosition;
        private bool _isCardSettingUp;
        private CardDeck _cardDeck;

        public virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();
            _isCardSettingUp = false;
        }

        void OnEnable()
        {
            SetCardDeck();
        }

        private void Update()
        {
            Vector2 mousePos = Input.mousePosition;
            
            if (Input.GetMouseButtonUp(0) 
                && RectTransformUtility.RectangleContainsScreenPoint(_rectTransform, mousePos)
                && !_isCardSettingUp)
            {
                GameObject currentPlayer = CharacterSelector.CurrentCharacter.gameObject;
                
                if (_cardDeck.GetApproval(gameObject) && currentPlayer.TryGetComponent(out T action))
                {
                    _isCardSettingUp = true;
                    _canvasGroup.alpha = 0.5f;
                    OnCardActivation(action);
                }
            }
        }

        public abstract void OnCardActivation(T arg1);

        public void OnCardSetUp(bool successfully)
        {
            if (successfully)
            {
                Pool();
            }

            _isCardSettingUp = false;
            _canvasGroup.alpha = 1f;
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
