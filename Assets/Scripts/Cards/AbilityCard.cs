using Characters.Skills;
using UnityEngine;

namespace Cards
{
    public class AbilityCard : Card
    {
        public override void OnCardActivation(GameObject activator)
        {
            if (activator.TryGetComponent(out SkillSelector selector))
            {
                selector.Select(OnCardSetUp);
            }
        }

        public override bool SingletonUse()
        {
            return true;
        }
    }
}
