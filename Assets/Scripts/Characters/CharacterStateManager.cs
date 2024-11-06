using System;
using Characters.CharacterStates;
using UnityEngine;

namespace Characters
{
    public class CharacterStateManager : MonoBehaviour
    {
        public Action<CharacterState> _onStateChanged;
        private CharacterState _currentState;

        private void Awake()
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
            _onStateChanged?.Invoke(newState);
        }

    }
}