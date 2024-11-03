using Collectibles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class EatCard : Card
    {
        public override void OnCardActivation(GameObject activator)
        {
            if (activator.TryGetComponent(out Inventory collector))
            {
                collector.PickUp(OnCardSetUp);
            }
        }

        public override bool SingletonUse()
        {
            return true;
        }
    }
}
