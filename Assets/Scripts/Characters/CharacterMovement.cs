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
        [SerializeField,Range(0,100)] private int steps = 0;
        [SerializeField, Range(0, 360f)] private float rotationStep = 90f;
        [SerializeField, ReadOnly] private Vector3 directionNormalized;
        [SerializeField] private GameObject HUD_display;
        [SerializeField] private bool isPaused;
        [SerializeField] private PathValidator pathValidator;

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

        public void AddSteps(int value)
        {
            steps += value;
        }

        public void SetPaused (bool state)
        {
            isPaused = state;
        }

        private void OnTick()
        {
        }

        public void MakeCustomRotationMovement(Vector3 nextPosition, bool isStepsIgnored = false) 
        {
            if (isPaused) return;


            if (steps > 0 || isStepsIgnored)
            {
                if (pathValidator.CanMoveTo(transform.position, nextPosition))
                {
                    Vector3 directionUnit = 
                        CoordinateManager.GetUnitDirection(pathValidator.GetTileMap(), transform.position, nextPosition);

                    while (!CoordinateManager.IsSameCell(transform.position, nextPosition)) {
                        if (makeStep(transform.position + directionUnit, true)) {
                            return;
                        }
                    }
                }
                else
                {
                    Debug.Log("Unable to move");
                }
            }
        }

        public void MakeMovement() 
        {
            if (isPaused) return;
            if (steps > 0)
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

        private bool makeStep(Vector3 nextPosition, bool isStepsIgnored = false) {
            transform.position = nextPosition;

            return decreaseStep();
        }

        private bool decreaseStep() {
            steps--;
            if (steps == 0) {
                EventManager.FireEvent(EventManager.OnTurnEnd);
                return true;
            } else {
                return false;
            }
        }

        public PathValidator GetPathValidator() {
            return pathValidator;
        }
    }
}
