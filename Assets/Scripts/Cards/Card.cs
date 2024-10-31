using Characters;
using Characters.CharacterStates;
using Collectibles;
using General;
using Managers;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cards
{
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public abstract class Card : MonoBehaviour, IPointerClickHandler
    {
        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;
        private Vector2 _startPosition;
        private CharacterStateManager _stateManager;
        private GameObject _cardOwner;
        private ActionBlockerManager actionBlocker;

        public virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();
            actionBlocker = FindObjectOfType<ActionBlockerManager>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            TryActivate();
        }

        public abstract void OnCardActivation(GameObject arg1);

        public void OnCardSetUp(bool successfully)
        {
            _canvasGroup.alpha = 1f;
            if (NetworkClient.connection.identity != null && NetworkClient.connection.identity.TryGetComponent(out CharacterStateManager stateManager))
            {
                if (successfully)
                {
                    stateManager.CmdSetCurrentState(new CardUsed());
                }
                else
                {
                    stateManager.CmdSetCurrentState(new Idle());
                }
            }
        }

        public void TryActivate()
        {
            if (actionBlocker.IsActionBlocked("CardUse"))
            {
                Debug.Log("Card usage is blocked!");
                return;
            }

            if (_stateManager.GetCurrentState().IsCardUsable(this) || 
                _stateManager.GetCurrentState().GetType() == typeof(MultiCard))
            if (NetworkClient.connection.identity != null && NetworkClient.connection.identity.TryGetComponent(out CharacterStateManager stateManager))
            {
                if (stateManager.GetCurrentState().IsCardUsable(this) ||
                stateManager.GetCurrentState().GetType() == typeof(MultiCard))
                {
                    _canvasGroup.alpha = 0.5f;

                    if (stateManager.GetCurrentState().IsCardUsable(this))
                    {
                        stateManager.CmdSetCurrentState(new CardSettingUp());
                        OnCardActivation(stateManager.gameObject);
                        return;
                    }

                    stateManager.CmdSetCurrentState(new CardSettingUp());
                }
            }
        }
        
        public static bool AttackAndEatAtCell(Vector3 cell, Attack attack, Inventory collector)
        {
            bool result = false;
            
            foreach (GameObject entity in CharacterMovement.GetEntities(cell))
            {
                if (entity.TryGetComponent(out Inventory _))
                {
                    attack.TryAttack(cell);
                    result = true;
                }
                else if (entity.TryGetComponent(out ICollectible collectible))
                {
                    if (collectible.GetType() == typeof(Human))
                    {
                        collector.PickUp(cell);
                        result = true;
                    }
                }
            }

            return result;
        }
    }
}
