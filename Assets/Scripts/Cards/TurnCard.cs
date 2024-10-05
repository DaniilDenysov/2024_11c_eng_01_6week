using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class TurnCard : Card<ITurnAction>
    {
        public override void OnCardActivation(ITurnAction arg1)
        {
            arg1.OnTurn();
        }
    }
}
