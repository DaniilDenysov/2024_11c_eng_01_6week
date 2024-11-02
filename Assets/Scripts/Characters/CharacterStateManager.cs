using System;
using Characters.CharacterStates;
using UnityEngine;

namespace Characters
{
    public class CharacterStateManager : MonoBehaviour
    {
        public Action<CharacterState> OnStateChanged;
        private CharacterState _currentState;

        private void Start()
        {
            CmdSetCurrentState(new Idle());
        }

        public void OnTurn()
        {
            _currentState = new Idle();
        }

        public CharacterState GetCurrentState()
        {
            return _currentState;
        }
        
        public void CmdSetCurrentState(CharacterState newState)
        {
            _currentState = newState;
            OnStateChanged?.Invoke(newState);
        }
    }
}