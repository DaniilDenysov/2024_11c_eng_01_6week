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

namespace Characters
{
    [RequireComponent(typeof(CharacterStateManager),typeof(ClientData))]
    public class CharacterMovement : MonoBehaviour, ITurnAction
    {
        [SerializeField, ReadOnly] private Vector3 directionNormalized;
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
            directionNormalized = Vector3.left;
            clientData = GetComponent<ClientData>();
            
            _stateManager.OnStateChanged += OnStateChanged;
           // EventManager.OnClientStartTurn += OnTurn;

        }

        private void Start()
        {
            InputManager.Instance.Subscribe(OnCellSelected);
        }

        private void OnCellSelected(InputAction.CallbackContext obj)
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
                HighlightAvailableMoves();
            }
        }

        public void HighlightAvailableMoves()
        {
            List<Vector3> litPositions = new List<Vector3>();
            int distance = 1;
            
            for (int availableSteps = 1; availableSteps < clientData.GetScoreAmount() + 1; availableSteps++)
            {
                Vector3 nextPosition = transform.position + directionNormalized * distance;
                
                if (IsSlimed(nextPosition))
                {
                    availableSteps++;

                    if (availableSteps >= clientData.GetScoreAmount() + 1)
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
                _onMoveAvailable.Invoke();
                HighlightDrawer.Instance.HighlightCells(litPositions);
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

        public void ChooseNewDirection(Action onDirectionChosen)
        {
            CharacterState previousState = _stateManager.GetCurrentState();
            _stateManager.CmdSetCurrentState(new CardSettingUp());

             TileSelector.Instance.SetDirectionsTilesLit(transform.position, cell =>
             {
                 OnDirectionChosen(onDirectionChosen);
             });
            HighlightDrawer.Instance.HighlightCells(GetAllDirections().Select((vec)=>transform.position + vec).ToList());
        }

        public void OnDirectionChosen (Action onDirectionChosen)
        {
            var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Vector2.Distance(transform.position, worldPoint) > 3) return;
            worldPoint.z = 0;
            var direction = (worldPoint - transform.position).normalized;
            directionNormalized = new Vector3Int(
                Mathf.RoundToInt(direction.x),
                Mathf.RoundToInt(direction.y),
                0
            );
            onDirectionChosen?.Invoke();
            HighlightDrawer.Instance.ClearHighlightedCells();
            Debug.Log("Direction changed");
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
