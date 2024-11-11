using System;
using Characters.Skills;
using UnityEngine;

namespace Cards
{
    public class AbilityCard : Card
    {
        private Action _onDiscard;
        private GameObject _lastActivator;
        
        public override void OnCardActivation(GameObject activator)
        {
            if (activator.TryGetComponent(out SkillSelector selector))
            {
                selector.Select(OnCardSetUp);
                _lastActivator = activator;
            }
        }

        public override void DiscardMove()
        {
            base.DiscardMove();
            _onDiscard?.Invoke();
            if (_lastActivator != null && _lastActivator.TryGetComponent(out SkillSelector selector))
            {
                selector.DiscardSkill();
            }
        }
    }
}
