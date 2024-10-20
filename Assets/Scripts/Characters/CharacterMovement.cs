using Cards;
using CustomTools;
using Ganeral;
using Managers;
using Selectors;
using System;
using System.Collections;
using System.Collections.Generic;
using Characters.CharacterStates;
using Selectors;
using UnityEngine;
using Validation;
using Mirror;
using Traps;
using UnityEngine.Tilemaps;

namespace Characters
{
    [RequireComponent(typeof(CharacterStateManager))]
    public class CharacterMovement : NetworkBehaviour, ITurnAction
    {
        [SerializeField, Range(0, 100)] private int steps = 0;
        [SerializeField, ReadOnly] private Vector3 directionNormalized;
        [SerializeField] private GameObject _sprite;
        
        [SerializeField] private GameObject HUD_display;
        [SerializeField] private PathValidator pathValidator;

        private CharacterStateManager _stateManager;
        private const int StepCost = 1;

        private void Awake()
        {
            _stateManager = GetComponent<CharacterStateManager>();
            EventManager.OnTick += OnTick;
            directionNormalized = Vector3.left;

            _stateManager._onStateChanged += OnStateChanged;
        }

        void Start()
        {
            CharacterSelector.Instance.RegisterCharacter(this);
        }

        public void OnTurn()
        {
            ChooseNewDirection(() => { });
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

        private void HighlightAvailableMoves()
        {
            List<Vector3> litPositions = new List<Vector3>();
            int distance = 1;
            
            for (int availableSteps = 1; availableSteps < steps + 1; availableSteps++)
            {
                Vector3 currentPosition = transform.position + directionNormalized * distance;
                
                if (IsSlimed(currentPosition))
                {
                    availableSteps++;

                    if (availableSteps >= steps + 1)
                    {
                        break;
                    }
                }
                
                if (pathValidator.CanMoveTo(transform.position, currentPosition))
                {
                    litPositions.Add(currentPosition);
                }
                else
                {
                    break;
                }

                distance++;
            }

            if (litPositions.Count > 0)
            {
                TileSelector.Instance.SetTilesLit(litPositions, MakeMovement);
            }
        }

        private void UnhighlightAvailableMoves()
        {
            TileSelector.Instance.SetTilesUnlit();
        }

        public void SetSteps(int value)
        {
            steps = value;
        }

        private void OnTick()
        {
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
            
            _stateManager.SetCurrentState(new Idle());
        }

        public void Teleport(Vector3 nextPosition)
        {
            _stateManager.SetCurrentState(new Idle());
            transform.position = nextPosition;
        }

        public void MakeMovement()
        {
            if (IsStepsEnough() && _stateManager.GetCurrentState().IsMovable())
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

        public void ChooseNewDirection(Action onDirectionChosen)
        {
            CharacterState previousState = _stateManager.GetCurrentState();
            _stateManager.SetCurrentState(new CardSettingUp());
            
            TileSelector.Instance.SetDirectionsTilesLit(transform.position, cell =>
            {
                OnDirectionChosen(cell, previousState, onDirectionChosen);
            });
        }

        private void OnDirectionChosen(Vector3 position, CharacterState previousState, Action onDirectionChosen)
        {
            directionNormalized = position - transform.position;
            float angle = Vector3.Angle(Vector3.up, directionNormalized);
            
            _sprite.transform.Rotate(new Vector3(0, 0, angle), Space.World);
            _stateManager.SetCurrentState(previousState);
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

        private bool IsStepsEnough()
        {
            return steps - StepCost >= 0;
        }

        public void DecreaseStep()
        {
            steps -= StepCost;
            if (steps == 0)
            {
                CharacterSelector.FinishTurn();
            }
        }

        public List<CharacterMovement> GetAvailableTargets(List<CharacterMovement> allCharacters)
        {
            List<CharacterMovement> targets = new List<CharacterMovement>(allCharacters);
            targets.Remove(this);
            return targets;
        }


        public PathValidator GetPathValidator()
        {
            return pathValidator;
        }

        public int GetSteps()
        {
            return steps;
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
