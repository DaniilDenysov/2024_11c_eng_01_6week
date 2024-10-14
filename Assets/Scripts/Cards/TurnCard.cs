using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class TurnCard : Card
    {
        public override void OnCardActivation(GameObject activator)
        {
            activator.TryGetComponent(out ITurnAction turnAction);
            turnAction.OnTurn();
            OnCardSetUp(true);
        }
    }
}
