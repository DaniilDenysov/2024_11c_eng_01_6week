using System;
using Cards;
using UnityEngine;

namespace Characters.Skills
{
    [RequireComponent(typeof(ITurnAction))]
    public class Changeability : Skill
    {
        private ITurnAction _turnAction;

        private void Awake()
        {
            _turnAction = GetComponent<ITurnAction>();
        }

        public override void Activate(Action<bool> onSetUp)
        {
            base.Activate(onSetUp);
            
            _turnAction.ChooseNewDirection(() =>
            {
                OnActivated();
            });
        }

        public override bool IsActivatable()
        {
            return true;
        }
    }
}