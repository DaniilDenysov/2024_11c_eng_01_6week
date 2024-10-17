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
using UnityEngine.Tilemaps;

namespace Characters
{
    [RequireComponent(typeof(CharacterStateManager))]
    public class CharacterMovement : NetworkBehaviour, ITurnAction
    {
        [SerializeField, Range(0, 100)] private int steps = 0;
        [SerializeField, Range(0, 360f)] private float rotationStep = 90f;
        [SerializeField, ReadOnly] private Vector3 directionNormalized;
        
        [SerializeField] private GameObject HUD_display;
        [SerializeField] private PathValidator pathValidator;

        private CharacterStateManager _stateManager;
        private int _stepCost = 1;

        private void Awake()
        {
            _stateManager = GetComponent<CharacterStateManager>();
            EventManager.OnTick += OnTick;
            directionNormalized = Vector3.left;
        }

        void Start()
        {
            CharacterSelector.Instance.RegisterCharacter(this);
        }

        public void OnTurn()
        {
            ChooseNewDirection(() => { });
        }

        public void RotateObject()
        {
            Vector2 directionContinuous = directionNormalized;
            Quaternion rotation = Quaternion.Euler(0, 0, rotationStep);
            directionContinuous = rotation * directionContinuous;
            directionNormalized = new Vector3(Mathf.RoundToInt(directionContinuous.x), Mathf.RoundToInt(directionContinuous.y));
        }

        public void SetSteps(int value)
        {
            steps = value;
        }

        private void OnTick()
        {
        }

        public void MakeCustomRotationMovement(Vector3 nextPosition, bool isStepsIgnored = false)
        {
            if (isStepsEnough() || isStepsIgnored)
            {
                if (pathValidator.CanMoveTo(transform.position, nextPosition))
                {
                    Vector3 directionUnit =
                        CharacterMovement.GetUnitDirection(pathValidator.GetTileMap(), 
                            transform.position, nextPosition);

                    while (transform.position != nextPosition)
                    {
                        if (!isStepsEnough() && !isStepsIgnored)
                        {
                            return;
                        }

                        makeStep(transform.position + directionUnit, isStepsIgnored);
                    }
                }
                else
                {
                    Debug.Log("Unable to move");
                }
            }
        }

        public void Teleport(Vector3 nextPosition)
        {
            _stateManager.SetCurrentState(new Idle());
            transform.position = nextPosition;
        }

        public void MakeMovement()
        {
            if (isStepsEnough() && _stateManager.GetCurrentState().IsMovable())
            {
                Vector3 nextPosition = transform.position + directionNormalized;
                if (pathValidator.CanMoveTo(nextPosition,
                    -new Vector3Int((int)directionNormalized.x, (int)directionNormalized.y, (int)directionNormalized.z)))
                {
                    makeStep(nextPosition);
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
            List<Vector3> turnPositions = CharacterMovement.GetAllDirections();

            for (int i = 0; i < turnPositions.Count; i++)
            {
                turnPositions[i] += transform.position;
            }

            TileSelector.Instance.SetTilesLit(turnPositions, cell =>
            {
                OnDirectionChosen(cell, previousState, onDirectionChosen);
            });
        }

        private void OnDirectionChosen(Vector3 position, CharacterState previousState, Action onDirectionChosen)
        {
            directionNormalized = position - transform.position;
            _stateManager.SetCurrentState(previousState);
            onDirectionChosen.Invoke();
        }

        private void makeStep(Vector3 nextPosition, bool isStepsIgnored = false)
        {
            if (!isStepsIgnored) {
                decreaseStep();
            }
            EventManager.FireEvent(EventManager.OnCharacterMovesOut, transform.position, this);
            transform.position = nextPosition;
            _stateManager.SetCurrentState(new Idle());
            EventManager.FireEvent(EventManager.OnCharacterMovesIn, nextPosition, this);
        }

        private bool isStepsEnough()
        {
            return steps - _stepCost >= 0;
        }

        private void decreaseStep()
        {
            steps -= _stepCost;
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

        public void SetStepCost(int stepCost)
        {
            _stepCost = stepCost;
        }

        public int GetSteps()
        {
            return steps;
        }
        
        public void ResetStepCost()
        {
            _stepCost = 1;
        }

        public Vector3 GetDirection()
        {
            return directionNormalized;
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

        public static Vector3 GetUnitDirection(Tilemap tilemap, Vector3 firstPosition, Vector3 secondPosition) {
            Vector3 unitSize = tilemap.layoutGrid.cellSize;

            Vector3Int firstTilePosition = tilemap.WorldToCell(firstPosition);
            Vector3Int secondTilePosition = tilemap.WorldToCell(secondPosition);
            Vector3Int vectorDifference = secondTilePosition - firstTilePosition;

            return new Vector3(
                vectorDifference.x > 0 ? unitSize.x : (vectorDifference.x < 0 ? -unitSize.x : 0),
                vectorDifference.y > 0 ? unitSize.y : (vectorDifference.y < 0 ? -unitSize.y : 0),
                0
            );
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
