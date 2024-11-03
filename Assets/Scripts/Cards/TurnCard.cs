using Characters;
using Distributors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class TurnCard : Card
    {
        public override void OnCardActivation(GameObject activator)
        {
            if (activator.TryGetComponent(out CharacterMovement turnAction))
            {
                turnAction.OnChooseNewDirection();
            }

            /*turnAction.ChooseNewDirection(() =>
            {
                OnCardSetUp(true);
            });*/
        }

        public override bool SingletonUse()
        {
            return true;
        }
    }
}
