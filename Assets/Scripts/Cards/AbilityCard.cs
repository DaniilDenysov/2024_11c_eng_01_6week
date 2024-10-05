using Characters.Skills;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class AbilityCard : Card<SkillSelector>
    {
        public override bool OnCardActivation(SkillSelector arg1)
        {
            return true;
            //selector
        }
    }
}
