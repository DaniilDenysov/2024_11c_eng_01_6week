using Cards;
using CustomTools;
using Managers;
using Selectors;
using System;
using System.Collections.Generic;
using Characters.CharacterStates;
using UnityEngine;
using Validation;
using Traps;
using UnityEngine.Events;
using Client;
using Mirror;
using UnityEngine.Serialization;

namespace Characters
{
    [RequireComponent(typeof(CharacterStateManager),typeof(ClientData))]
    public class CharacterMovement : MonoBehaviour, ITurnAction
    {
        [SerializeField, ReadOnly] private Vector3 directionNormalized;
        
        [SerializeField] private PathValidator pathValidator;
        public UnityEvent onMoveCancelable;
        public UnityEvent onMoveAvailable;

        private CharacterStateManager _stateManager;
        private const int StepCost = 1;
        private ClientData _clientData;
        private int _localScoreCount;

        private ActionBlockerManager actionBlocker;

        private void Awake()
        {
            _localScoreCount = 0;
            _stateManager = GetComponent<CharacterStateManager>();
            directionNormalized = Vector3.left;
            _clientData = GetComponent<ClientData>();
            
            _stateManager._onStateChanged += OnStateChanged;

            actionBlocker = FindObjectOfType<ActionBlockerManager>();
            EventManager.OnClientStartTurn += OnTurn;
        }

        public void OnTurn()
        {
            if (_clientData.GetTurn())
            {
                _localScoreCount = _clientData.GetScoreAmount();
                _stateManager.OnTurn();
                ChooseNewDirection(() => { }, null);
            }
            else
            {
                onMoveAvailable.Invoke();
            }
        }

        private void OnStateChanged(CharacterState newState)
        {
            if (newState.IsMovable())
            {
                HighlightAvailableMoves();
            }
            else
            {
                UnhighlightAvailableMoves();
            }
        }

        [Button]
        public void HighlightAvailableMoves()
        {
            List<Vector3> litPositions = new List<Vector3>();
            int distance = 1;
            
            for (int availableSteps = 1; availableSteps < _localScoreCount + 1; availableSteps++)
            {
                Vector3 nextPosition = transform.position + directionNormalized * distance;
                
                if (IsSlimed(nextPosition))
                {
                    availableSteps++;

                    if (availableSteps >= _localScoreCount + 1)
                    {
                        break;
                    }
                }
                
                if (pathValidator.CanMoveTo(transform.position, nextPosition))
                {
                    litPositions.Add(nextPosition);
                }
 
                distance++;
            }

            if (litPositions.Count > 0)
            {
                onMoveAvailable.Invoke();
                TileSelector.Instance.SetTilesLit(litPositions, MakeMovement);
            }
            else if (_clientData.GetTurn())
            {
                onMoveCancelable.Invoke();
            }
        }

        private void UnhighlightAvailableMoves()
        {
            TileSelector.Instance.SetTilesUnlit();
        }
        
        public void MakeMovement(Vector3 nextPosition)
        {
            Vector3 difference = nextPosition - transform.position;
            int steps = (int)Math.Abs(difference.x);

            if (difference.x != 0 && difference.y != 0)
            {
                Debug.LogError("Unable to move into such direction");
            }
            else if (difference.x == 0)
            {
                steps = (int)Math.Abs(difference.y);
            }

            for (int i = 0; i < steps; i++) {
                MakeMovement();
            }
            
            _stateManager.CmdSetCurrentState(new Idle());
        }

        public void MakeMovement()
        {
            if (_stateManager.GetCurrentState().IsMovable())
            {
                Vector3 nextPosition = transform.position + directionNormalized;
                
                if (pathValidator.CanMoveTo(nextPosition,
                    -new Vector3Int((int)directionNormalized.x, (int)directionNormalized.y, (int)directionNormalized.z)))
                {
                    MakeStep(nextPosition);
                }
                else
                {
                    Debug.Log("Unable to move");
                }
            }
        }

        public void ChooseNewDirection(Action onDirectionChosen, Card card)
        {
            CharacterState previousState = _stateManager.GetCurrentState();
            _stateManager.CmdSetCurrentState(new CardSettingUp(card));
            
            TileSelector.Instance.SetDirectionsTilesLit(transform.position, cell =>
            {
                OnDirectionChosen(cell, previousState, onDirectionChosen);
            });
        }

        private void OnDirectionChosen(Vector3 position, CharacterState previousState, Action onDirectionChosen)
        {
            directionNormalized = position - transform.position;
            _stateManager.CmdSetCurrentState(previousState);
            onDirectionChosen.Invoke();
        }

        private void MakeStep(Vector3 nextPosition, bool isStepsIgnored = false)
        {
            if (!isStepsIgnored) {
                DecreaseStep();
            }
            EventManager.FireEvent(EventManager.OnCharacterMovesOut, transform.position, this);
            transform.position = nextPosition;
            EventManager.FireEvent(EventManager.OnCharacterMovesIn, nextPosition, this);
        }

        public void DecreaseStep()
        {
            _localScoreCount -= StepCost;
            _clientData.CmdSetScoreAmount(_localScoreCount);
            
            if (_localScoreCount <= 0)
            {
                EventManager.FireEvent(EventManager.OnTurnFinished);
            }
        }

        public PathValidator GetPathValidator()
        {
            return pathValidator;
        }
        
        public static List<GameObject> GetEntities(Vector3 position, GameObject self = null)
        {
            var circles = Physics2D.OverlapCircleAll(position, 0.1f);

            List<GameObject> result = new List<GameObject>();
            foreach (Collider2D circle in circles) {
                if (circle != default && circle.gameObject != self) {
                    result.Add(circle.gameObject);
                }
            }

            return result;
        }

        private bool IsSlimed(Vector3 position)
        {
            foreach (SlimeTrail slimeGameObject in GetEntityOfType<SlimeTrail>(position))
            {
                if (slimeGameObject.GetOwner() != gameObject)
                {
                    return true;
                }
            }
            
            return false;
        }
        
        public static List<T> GetEntityOfType<T>(Vector3 position)
        {
            var circles = Physics2D.OverlapCircleAll(position, 0.1f);

            List<T> result = new List<T>();
            foreach (Collider2D circle in circles) {
                if (circle != default && circle.gameObject.TryGetComponent(out T component)) {
                    result.Add(component);
                }
            }

            return result;
        }

        public static List<Vector3> GetAllDirections() {
            return new List<Vector3> {
                new Vector3(0, 1),
                new Vector3(1, 0),
                new Vector3(0, -1),
                new Vector3(-1, 0)
            };
        }
        
        public static List<Vector3> GetAttackDirections() {
            return new List<Vector3> {
                new Vector3(0, 1),
                new Vector3(1, 0),
                new Vector3(0, -1),
                new Vector3(-1, 0),
                new Vector3(0, 0)
            };
        }

        public static Vector3Int VectorToIntVector(Vector3 vector) {
            return new Vector3Int((int)vector.x, (int)vector.y, 0);
        }

        public static Vector3Int NormalizeIntVector(Vector3Int vector) {
            float threshold = 1;

            return new Vector3Int(
                vector.x >= threshold ? 1 : vector.x <= -threshold ? -1 : 0,
                vector.y >= threshold ? 1 : vector.y <= -threshold ? -1 : 0,
                0
            );
        }
    }
}
