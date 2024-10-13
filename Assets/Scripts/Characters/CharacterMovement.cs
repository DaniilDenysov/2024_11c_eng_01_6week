using Cards;
using CustomTools;
using Ganeral;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Validation;

namespace Characters
{
    public class CharacterMovement : MonoBehaviour, ITurnAction
    {
        [SerializeField, Range(0, 100)] private int steps = 0;
        [SerializeField, Range(0, 360f)] private float rotationStep = 90f;
        [SerializeField, ReadOnly] private Vector3 directionNormalized;
        [SerializeField] private GameObject HUD_display;
        [SerializeField] private bool isPaused;
        [SerializeField] private PathValidator pathValidator;
        private int _stepCost = 1;

        private void Awake()
        {
            EventManager.OnTick += OnTick;
            directionNormalized = Vector3.left;
        }

        public void OnTurn()
        {
            HUD_display.SetActive(true);
            isPaused = true;
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

        public void SetPaused(bool state)
        {
            isPaused = state;
        }

        private void OnTick()
        {
        }

        public void MakeCustomRotationMovement(Vector3 nextPosition, bool isStepsIgnored = false)
        {
            if (isPaused) return;

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

        public void Teleport(Vector3 nextPosition, bool isStepsIgnored = false)
        {
            if (isPaused) return;

            if (steps > 0 || isStepsIgnored)
            {
                Vector3 directionUnit =
                    CoordinateManager.GetUnitDirection(pathValidator.GetTileMap(), transform.position, nextPosition);

                while (!CoordinateManager.IsSameCell(transform.position, nextPosition))
                {
                    if (!isStepsEnough() && !isStepsIgnored)
                    {
                        return;
                    }

                    makeStep(transform.position + directionUnit, isStepsIgnored);
                }
            }
        }

        public void MakeMovement()
        {
            if (isPaused) return;
            if (isStepsEnough())
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
                EventManager.FireEvent(EventManager.OnTurnEnd);
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
    }
}
