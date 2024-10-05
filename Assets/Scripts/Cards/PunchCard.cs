using Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class PunchCard : Card<Attack>
    {
        public override bool OnCardActivation(Attack arg1)
        {
            return arg1.TryAttack();
        }
    }
}
