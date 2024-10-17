using Characters;
using Characters.CharacterStates;
using General;
using Selectors;
using UnityEngine;

namespace Cards
{
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public abstract class Card : CardPoolable
    {
        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;
        private Vector2 _startPosition;
        private CharacterStateManager _stateManager;
        private GameObject _cardOwner;

        public virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();
        }

        public void SetUp(GameObject player, CharacterStateManager stateManager)
        {
            _cardOwner = player;
            _stateManager = stateManager;
        }

        private void Update()
        {
            Vector2 mousePos = Input.mousePosition;
            
            if (Input.GetMouseButtonUp(0) 
                && RectTransformUtility.RectangleContainsScreenPoint(_rectTransform, mousePos))
            {
                TryActivate();
            }
        }

        public abstract void OnCardActivation(GameObject arg1);

        public void OnCardSetUp(bool successfully)
        {
            _canvasGroup.alpha = 1f;
            
            if (successfully)
            {
                _stateManager.SetCurrentState(new CardUsed());
                Pool();
            }
            else
            {
                _stateManager.SetCurrentState(new Idle());
            }
        }

        public void TryActivate()
        {
            if (_stateManager.GetCurrentState().IsCardUsable(this) || 
                _stateManager.GetCurrentState().GetType() == typeof(MultiCard))
            {
                _canvasGroup.alpha = 0.5f;
                
                if (_stateManager.GetCurrentState().IsCardUsable(this))
                {
                    _stateManager.SetCurrentState(new CardSettingUp());
                    OnCardActivation(_cardOwner);
                    return;
                }
                
                _stateManager.SetCurrentState(new CardSettingUp());
            }
        }
    }
}
