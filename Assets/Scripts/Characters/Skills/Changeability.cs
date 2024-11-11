using System;
using Cards;
using Characters.CharacterStates;
using UnityEngine;

namespace Characters.Skills
{
    [RequireComponent(typeof(ITurnAction)), RequireComponent(typeof(CharacterStateManager))]
    public class Changeability : Skill
    {
        private ITurnAction _turnAction;
        private CharacterStateManager _stateManager;

        private void Awake()
        {
            _turnAction = GetComponent<ITurnAction>();
            _stateManager = GetComponent<CharacterStateManager>();
        }

        public override void Activate(Action<bool> onSetUp)
        {
            base.Activate(onSetUp);

            Card _currentCard = null;
            
            if (_stateManager.GetCurrentState().GetType() == typeof(CardSettingUp))
            {
                CardSettingUp state = (CardSettingUp)_stateManager.GetCurrentState();
                
                _currentCard = state.GetCard();
            }
            else
            {
                Debug.LogError("Changebility not set up correctly");
            }
            
            _turnAction.ChooseNewDirection(() =>
            {
                OnActivated();
            }, _currentCard);
        }

        public override bool IsActivatable()
        {
            return true;
        }
    }
}