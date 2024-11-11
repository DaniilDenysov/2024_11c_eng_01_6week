using System;
using System.Collections.Generic;
using System.Linq;
using Cards;
using Characters.CharacterStates;
using Collectibles;
using Mirror;
using Traps;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Button = UnityEngine.UI.Button;

namespace Characters.Skills
{
    [RequireComponent(typeof(CharacterMovement)), 
     RequireComponent(typeof(CharacterStateManager)), 
     RequireComponent(typeof(Attack)), 
     RequireComponent(typeof(Inventory))]
    public class Flexibility : Skill
    {
        [SerializeField] private HorntipedeBody bodyPrefab;
        [SerializeField] private UnityEvent<Action> onBecameCancelable;
        [SerializeField] private UnityEvent OnDiscarded;
        
        private CharacterMovement _movement;
        private CharacterStateManager _stateManager;
        private Attack _attack;
        private Inventory _collector;

        private Card _usedCard;
        private Card _currentCard;
        
        private readonly List<HorntipedeBody> _body = new();
        private const int BodyLength = 2;
        
        private void Awake()
        {
            _movement = GetComponent<CharacterMovement>();
            _stateManager = GetComponent<CharacterStateManager>();
            _attack = GetComponent<Attack>();
            _collector = GetComponent<Inventory>();
        }

        public override void Activate(Action<bool> onSetUp)
        {
            base.Activate(onSetUp);

            if (_stateManager.GetCurrentState().GetType() == typeof(CardSettingUp))
            {
                CardSettingUp state = (CardSettingUp)_stateManager.GetCurrentState();
                
                _currentCard = state.GetCard();
            }
            else
            {
                Debug.LogError("Flexibility not set up correctly");
            }

            HighlightAvailableTiles(transform.position);
        }

        private void OnCellChosen(Vector3 cell)
        {
            CmdSpawnBody(cell);
        }
        
        [Command(requiresAuthority = false)]
        private void CmdSpawnBody(Vector3 position)
        {
            HorntipedeBody body = Instantiate(bodyPrefab, position, transform.rotation);
            NetworkServer.Spawn(body.gameObject);
            RpcSpawnBody(body);
        }

        [TargetRpc]
        private void RpcSpawnBody(HorntipedeBody bodyCell)
        {
            _body.Add(bodyCell);
            bodyCell.SetUp(gameObject);
            
            if (_body.Count < BodyLength)
            {
                if (_body.Count < 2)
                {
                    HighlightAvailableTiles(transform.position);
                }
                else
                {
                    HighlightAvailableTiles(_body[^2].transform.position);
                }
            }
            else
            {
                onBecameCancelable.Invoke(ApplyMove);
                _stateManager.CmdSetCurrentState(new MultiCard(OnCardUsed, _currentCard));
            }
        }

        private void OnCardUsed(Card card)
        {
            if (card.GetType() == typeof(PunchCard) || card.GetType() == typeof(EatCard))
            {
                bool isPunchCard = card.GetType() == typeof(PunchCard);
                List<Vector3> litPositions = new List<Vector3>();

                foreach (HorntipedeBody bodyUnit in _body)
                {
                    if ((isPunchCard && bodyUnit.IsCellAttackable()) || 
                        (!isPunchCard && bodyUnit.IsCellEatable()))
                    {
                        litPositions.Add(bodyUnit.transform.position);
                    }
                }
            
                _stateManager.CmdSetCurrentState(new CardSettingUp(_currentCard));
                _usedCard = card;

                if (litPositions.Count < 1)
                {
                    card.OnCardSetUp(false);
                    _stateManager.CmdSetCurrentState(new MultiCard(OnCardUsed, _currentCard));
                    return;
                }
                
                TileSelector.Instance.SetTilesLit(litPositions, OnCardSetUp);
            }
            else
            {
                card.OnCardSetUp(false);
                Debug.Log("Wrong card was used");
            }
        }

        private void OnCardSetUp(Vector3 cell)
        {
            if (_usedCard.GetType() == typeof(PunchCard))
            {
                _usedCard.OnCardSetUp(_attack.TryAttack(cell));
            }
            else if (_usedCard.GetType() == typeof(EatCard))
            {
                _usedCard.OnCardSetUp(_collector.PickUp(cell));
            } 
            else
            {
                Debug.Log("Behaviour for card: " + _usedCard.name + " is not written");
            }
            
            _stateManager.CmdSetCurrentState(new MultiCard(OnCardUsed, _currentCard));
        }

        public void ApplyMove()
        {
            TileSelector.Instance.SetTilesLit(new List<Vector3>()
            {
                transform.position,
                _body.Last().transform.position
            }, OnShrinkPositionChosen);
        }

        public void OnShrinkPositionChosen(Vector3 cell)
        {
            ClearBody();
            
            transform.position = cell;
            _movement.ChooseNewDirection(() =>
            {
                OnActivated();
            }, _currentCard);
        }

        private void ClearBody()
        {
            foreach (HorntipedeBody bodyUnit in _body)
            {
                bodyUnit.CmdRemoveFromField();
            }
            _body.Clear();
        }

        public void HighlightAvailableTiles(Vector3 previousPos)
        {
            Vector3 currentCell = _body.Count > 0 ? _body.Last().transform.position : transform.position;
            List<Vector3> litPositions = _movement.GetPathValidator().GetAvailableCells(currentCell, 1);
            
            litPositions.Remove(previousPos);

            TileSelector.Instance.SetTilesLit(litPositions, OnCellChosen);
        }

        public override bool IsActivatable()
        {
            
            return true;
        }

        public override void OnDiscard()
        {
            base.OnDiscard();
            
            if (_usedCard != null)
            {
                _usedCard.OnCardSetUp(false);
            }
            
            OnDiscarded.Invoke();
            
            ClearBody();
        }
    }
}