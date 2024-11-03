using Characters;
using Characters.CharacterStates;
using Collectibles;
using Distributors;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cards
{
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public abstract class Card : MonoBehaviour, IPointerClickHandler
    {
        private CanvasGroup _canvasGroup;
        private Vector2 _startPosition;
        private Vector3 _activationPosition;

        public virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            TryActivate();
        }

        public abstract bool SingletonUse();

        public abstract void OnCardActivation(GameObject arg1);

        public void OnCardSetUp(bool successfully)
        {
            _canvasGroup.alpha = 1f;
            if (NetworkClient.connection.identity != null && NetworkClient.connection.identity.TryGetComponent(out CharacterStateManager stateManager))
            {
                if (successfully)
                {
                    stateManager.CmdSetCurrentState(
                        new CardUsed(_activationPosition != stateManager.gameObject.transform.position));
                }
                else
                {
                    stateManager.CmdSetCurrentState(new Idle());
                }
            }
        }

        public void TryActivate()
        {
            if (NetworkClient.connection.identity != null && NetworkClient.connection.identity.TryGetComponent(out CharacterStateManager stateManager))
            {
                if (stateManager.GetCurrentState().IsCardUsable(this) ||
                stateManager.GetCurrentState().GetType() == typeof(MultiCard))
                {
                    _canvasGroup.alpha = 0.5f;
                    _activationPosition = stateManager.gameObject.transform.position;
                    
                    if (stateManager.GetCurrentState().IsCardUsable(this))
                    {
                        stateManager.CmdSetCurrentState(new CardSettingUp());
                        if (CardDistributor.Instance.UsedCards.TryPeek(out Card card))
                        {
                            if (card != null)
                            {
                                card.Cancel();
                            }
                            CardDistributor.Instance.UsedCards.Pop();
                        }
                        CardDistributor.Instance.UsedCards.Push(this);
                        OnCardActivation(stateManager.gameObject);
                        return;
                    }
                    
                    stateManager.CmdSetCurrentState(new CardSettingUp());
                }
            }
        }

        public void Cancel()
        {
            OnCardSetUp(false);
        }

        public void Discard ()
        {
            OnCardSetUp(true);
            Destroy(gameObject);
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
