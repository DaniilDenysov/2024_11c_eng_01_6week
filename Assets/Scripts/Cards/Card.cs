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
        private String _initializedFromName;

        public virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (NetworkClient.connection.identity != null 
                    && NetworkClient.connection.identity.TryGetComponent(out CharacterStateManager stateManager))
                {
                    if (!stateManager.GetCurrentState().IsCardDiscardable(this))
                    {
                        TryActivate();
                    }
                    else
                    {
                        DiscardMove();
                    }
                }
            } 
            else if (eventData.button == PointerEventData.InputButton.Right)
                TryDiscardCard();
        }

        public abstract void OnCardActivation(GameObject arg1);

        public void OnCardSetUp(bool successfully)
        {
            _canvasGroup.alpha = 1f;
            
            if (NetworkClient.connection.identity != null 
                && NetworkClient.connection.identity.TryGetComponent(out CharacterStateManager stateManager))
            {
                if (successfully)
                {
                    ClientDeck.Instance.Remove(this);
                    CardDistributor.Instance.CmdDiscardCard(_initializedFromName);
                    
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
        
        public void TryDiscardCard()
        {
            if (NetworkClient.connection.identity != null 
                && NetworkClient.connection.identity.TryGetComponent(out ClientData clientData)
                && NetworkClient.connection.identity.TryGetComponent(out CharacterStateManager stateManager))
            {
                if (clientData.GetTurn() && stateManager.GetCurrentState().IsCardUsable(this))
                {
                    stateManager.CmdSetCurrentState(new CardUsed(false));
                    ClientDeck.Instance.Remove(this);
                    CardDistributor.Instance.CmdDiscardCard(_initializedFromName);
                    
                    gameObject.SetActive(false);
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
                    if (stateManager.GetCurrentState().IsCardUsable(this))
                    {
                        _canvasGroup.alpha = 0.5f;
                        _activationPosition = stateManager.gameObject.transform.position;

                        stateManager.CmdSetCurrentState(new CardSettingUp(this));
                        OnCardActivation(stateManager.gameObject);
                        return;
                    }

                    if (stateManager.GetCurrentState().GetType() == typeof(MultiCard))
                    {
                        MultiCard state = (MultiCard)stateManager.GetCurrentState();
                        
                        _canvasGroup.alpha = 0.5f;
                        _activationPosition = stateManager.gameObject.transform.position;
                        
                        state.OnCardActivation(this);
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

        public void SetInitializedFrom(String origin)
        {
            _initializedFromName = origin;
        }

        public virtual void DiscardMove()
        {
            _canvasGroup.alpha = 1f;
            TileSelector.Instance.DiscardSelection();
            
            if (NetworkClient.connection.identity != null 
                && NetworkClient.connection.identity.TryGetComponent(out CharacterStateManager stateManager))
            {
                stateManager.CmdSetCurrentState(new Idle());
            }
        }
    }
}
