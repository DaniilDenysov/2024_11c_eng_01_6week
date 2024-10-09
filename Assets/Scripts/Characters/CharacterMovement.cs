using Cards;
using CustomTools;
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

        public void MakeMovement() 
        {
            if (isPaused) return;
            if (steps > 0)
            {
                Vector3 nextPosition = transform.position + directionNormalized;
                if (pathValidator.CanMoveTo(nextPosition,
                    -new Vector3Int((int)directionNormalized.x, (int)directionNormalized.y, (int)directionNormalized.z)))
                {
                    transform.position = nextPosition;
                }
                else
                {
                    Debug.Log("Unable to move");
                }
                steps--;
                if (steps == 0) EventManager.FireEvent(EventManager.OnTurnEnd);
            }
        }

        public PathValidator GetPathValidator() {
            return pathValidator;
        }
    }
}
