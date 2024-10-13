using Characters.Skills;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class AbilityCard : Card<SkillSelector>
    {
        public override void OnCardActivation(SkillSelector arg1)
        {
            arg1.Select(OnCardSetUp);
        }
    }
}
