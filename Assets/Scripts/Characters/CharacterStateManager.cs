using System;
using Characters.CharacterStates;
using UnityEngine;

namespace Characters
{
    public class CharacterStateManager : MonoBehaviour
    {
        private CharacterState _currentState;

        private void Awake()
        {
            _currentState = new Idle();
        }

        public CharacterState GetCurrentState()
        {
            return _currentState;
        }
        
        public void SetCurrentState(CharacterState newState)
        {
            _currentState = newState;
        }
    }
}