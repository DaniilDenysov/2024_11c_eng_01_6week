using Cards;
using CustomTools;
using Ganeral;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using Characters.CharacterStates;
using Selectors;
using UnityEngine;
using Validation;
using Mirror;

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
                        CoordinateManager.GetUnitDirection(pathValidator.GetTileMap(), 
                            transform.position, nextPosition);

                    while (!CoordinateManager.IsSameCell(transform.position, nextPosition))
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
            transform.position = nextPosition;
        }

        public void MakeMovement()
        {
            if (isStepsEnough() && _stateManager.GetCurrentState().GetType() == typeof(Idle))
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
            List<Vector3> turnPositions = CoordinateManager.GetAllDirections();

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
    }
}
