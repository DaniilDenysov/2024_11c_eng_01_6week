using Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class PunchCard : Card<Attack>
    {
        public override void OnCardActivation(Attack arg1)
        {
            OnCardSetUp(arg1.TryAttack());
        }
    }
}
