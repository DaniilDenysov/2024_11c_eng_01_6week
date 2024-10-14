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
                if (_stateManager.GetCurrentState().OnCardUsed(this))
                {
                    _stateManager.SetCurrentState(new CardSetingUp());
                    _canvasGroup.alpha = 0.5f;
                    OnCardActivation(_cardOwner);
                }
            }
        }

        public abstract void OnCardActivation(GameObject arg1);

        public void OnCardSetUp(bool successfully)
        {
            _stateManager.SetCurrentState(new Idle());
            _canvasGroup.alpha = 1f;
            
            if (successfully)
            {
                Pool();
            }
        }
    }
}
