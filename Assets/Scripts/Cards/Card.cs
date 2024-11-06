using System;
using Characters;
using Characters.CharacterStates;
using Client;
using Collectibles;
using Distributors;
using Mirror;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cards
{
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public abstract class Card : NetworkBehaviour, IPointerClickHandler
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

        public abstract void OnCardActivation(GameObject arg1);

        public void OnCardSetUp(bool successfully)
        {
            _canvasGroup.alpha = 1f;
            if (NetworkClient.connection.identity != null && NetworkClient.connection.identity.TryGetComponent(out CharacterStateManager stateManager))
            {
                if (successfully)
                {
                    ClientDeck.Instance.Remove(this);
                    CardDistributor.Instance.CmdDiscardCard(this);
                    
                    stateManager.CmdSetCurrentState(
                        new CardUsed(_activationPosition != stateManager.gameObject.transform.position));
                    
                    gameObject.SetActive(false);
                }
                else
                {
                    stateManager.CmdSetCurrentState(new Idle());
                }
            }
        }

        public void TryActivate()
        {
            if (NetworkClient.connection.identity != null 
                && NetworkClient.connection.identity.TryGetComponent(out ClientData clientData)
                && NetworkClient.connection.identity.TryGetComponent(out CharacterStateManager stateManager))
            {
                if (clientData.GetTurn())
                {
                    if (stateManager.GetCurrentState().IsCardUsable(this) ||
                        stateManager.GetCurrentState().GetType() == typeof(MultiCard))
                    {
                        _canvasGroup.alpha = 0.5f;
                        _activationPosition = stateManager.gameObject.transform.position;

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
