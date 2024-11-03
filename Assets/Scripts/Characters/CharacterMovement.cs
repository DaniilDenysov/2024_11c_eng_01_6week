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
using UnityEngine.InputSystem;
using System.Linq;
using Extensions.Vector;
using Distributors;

namespace Characters
{
    [RequireComponent(typeof(CharacterStateManager),typeof(ClientData))]
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField, ReadOnly] private Vector3Int directionNormalized;
        [SerializeField] private GameObject _sprite;
        [SerializeField] private PathValidator pathValidator;
        public UnityEvent _onMoveCancelable;
        public UnityEvent _onMoveAvailable;

        
        private CharacterStateManager _stateManager;
        private const int StepCost = 1;
        private ClientData clientData;

        private void Awake()
        {
            _stateManager = GetComponent<CharacterStateManager>();
            foreach (var direction in GetAllDirections())
            {
                var nextPosition = transform.position + direction;
                Debug.Log($"Current: {transform.position} Direction: {direction} NextPosition: {nextPosition}");
                if (pathValidator.CanMoveTo(nextPosition, direction.VectorToIntVector()))
                {
                    directionNormalized = new Vector3Int((int)direction.x, (int)direction.y);
                    Debug.Log($"{name} + {direction}");
                    break;
                }
            }
            clientData = GetComponent<ClientData>();
            
            _stateManager.OnStateChanged += OnStateChanged;
        }

       // private void OnCellSelected(InputAction.CallbackContext obj)
        private void OnCellSelected(Vector3 obj)
        {
            if (!clientData.GetTurn()) return;
            if (!IsStepsEnough()) return;
            if (!_stateManager.GetCurrentState().IsMovable()) return;

            var nextPosition = transform.position + directionNormalized;
            Debug.Log($"Current: {transform.position} Direction: {directionNormalized} NextPosition: {nextPosition}");
            if (pathValidator.CanMoveTo(nextPosition, directionNormalized))
            {
                MoveTo(nextPosition);
                DecreaseStep();
                InputManager.Instance.ClearCallbacks();
                HighlightDrawer.Instance.ClearHighlightedCells();
                _stateManager.CmdSetCurrentState(new Idle());
            }
        }

      /*  public void OnTurn()
        {
            if (clientData.GetTurn())
            {
                _stateManager.OnTurn();
                ChooseNewDirection(() => { });
            }
        }*/

        private void OnStateChanged(CharacterState newState)
        {
            if (newState.IsMovable())
            {
                Debug.Log($"State changed: {newState.GetType()}");
                //HighlightMoves(1);
                HighlightAvailableMoves();
            }
        }

        public void HighlightMoves (int depth)
        {
            List<Vector3> litPositions = new List<Vector3>();
            int distance = 1;
            for (int availableSteps = 1; availableSteps < depth; availableSteps++)
            {
                Vector3 nextPosition = transform.position + (directionNormalized * distance);
                if (IsSlimed(nextPosition))
                {
                    availableSteps++;

                    if (availableSteps >= clientData.GetScoreAmount() + 1)
                    {
                        break;
                    }
                }

                if (pathValidator.CanMoveTo(nextPosition, directionNormalized * distance))
                {
                    litPositions.Add(nextPosition);
                }
                else
                {
                    Debug.Log("Not highlighted");
                }

                if (litPositions.Count > 0)
                {
                    _onMoveAvailable.Invoke();
                    Debug.Log("Highlighted");
                    HighlightDrawer.Instance.HighlightCells(litPositions);
                    InputManager.Instance.AddCellCallbacksOverride(new HashSet<Vector3>(litPositions), OnCellSelected);
                }
                else if (clientData.GetTurn())
                {
                    _onMoveCancelable.Invoke();
                }
                distance++;
            }
        }
         
        public void HighlightAvailableMoves()
        {
            List<Vector3> litPositions = new List<Vector3>();
            int distance = 1;
            int maxDistance = 2;
            //should be explicitly called when score added

            for (int i = 0;i<clientData.GetScoreAmount() && distance < maxDistance;i++,distance++)
            {
                Vector3 nextPosition = transform.position + (directionNormalized * distance);
                Debug.Log("NextPos:"+nextPosition);

                /*if (IsSlimed(nextPosition))
                {
                    availableSteps++;

                    if (availableSteps >= clientData.GetScoreAmount() + 1)
                    {
                        break;
                    }
                }*/

                if (pathValidator.CanMoveTo(nextPosition, directionNormalized))
                {
                    litPositions.Add(nextPosition);
                }
            }

            /*for (int availableSteps = 0; availableSteps < clientData.GetScoreAmount(); availableSteps++)
            {
                Vector3 nextPosition = transform.position + (directionNormalized * distance);
                Debug.Log("1");
                if (IsSlimed(nextPosition))
                {
                    availableSteps++;

                    if (availableSteps >= clientData.GetScoreAmount() + 1)
                    {
                        break;
                    }
                }
                
                if (pathValidator.CanMoveTo(nextPosition, directionNormalized))
                {
                    litPositions.Add(nextPosition);
                }
                else
                {
                    Debug.Log("Not highlighted");
                }
 
                distance++;
            }*/

            if (litPositions.Count > 0)
            {
                _onMoveAvailable.Invoke();
                Debug.Log("Highlighted");
               
                HighlightDrawer.Instance.HighlightCells(litPositions);
                var set = new HashSet<Vector3>(litPositions);
                Debug.Log("SetLen:"+set.Count);
                InputManager.Instance.AddCellCallbacksOverride(set, OnCellSelected);
            }
            else if (clientData.GetTurn())
            {
                _onMoveCancelable.Invoke();
            }
        }
        
        public void MoveTo(Vector3 nextPosition)
        {
            transform.position = nextPosition;
            _stateManager.CmdSetCurrentState(new Idle());
        }


        public void OnChooseNewDirection()
        {
            CharacterState previousState = _stateManager.GetCurrentState();
            _stateManager.CmdSetCurrentState(new CardSettingUp());
            List<Vector3> positions = GetAllDirections().Select((vec) => transform.position + vec).ToList();
            InputManager.Instance.AddCellCallbacksOverride(new HashSet<Vector3>(positions), OnNewDirectionChosen);
            HighlightDrawer.Instance.HighlightCells(positions);
        }

        public void OnNewDirectionChosen(Vector3 cell)
        {
          //  HashSet<Vector3> positions = new HashSet<Vector3>(GetAllDirections().Select((vec) => transform.position + vec).ToList());
            Debug.Log("Direction changed");

            var direction = (cell - transform.position).normalized;
            directionNormalized = new Vector3Int(
                Mathf.RoundToInt(direction.x),
                Mathf.RoundToInt(direction.y),
                0
            );
            Debug.DrawRay(cell, direction, Color.red, 5f);
            Debug.Log(direction + " " + directionNormalized);
            if (CardDistributor.Instance.UsedCards.TryPeek(out Card card))
            {
                card.Discard();
            }
            InputManager.Instance.ClearCallbacks();
            HighlightDrawer.Instance.ClearHighlightedCells();
            _stateManager.CmdSetCurrentState(new Idle());
        }

        private bool IsStepsEnough()
        {
            return clientData.GetScoreAmount() - StepCost >= 0;
        }

        public void DecreaseStep()
        {
            clientData.CmdSetScoreAmount(clientData.GetScoreAmount() - StepCost);
            //move to client data
          /*  if (clientData.GetScoreAmount() == 0)
            {
                EventManager.FireEvent(EventManager.OnTurnFinished);
            }*/
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

        public bool IsSlimed(Vector3 position)
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
    }
}
