using Collectibles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class EatCard : Card<ICollector>
    {
        public override void OnCardActivation(ICollector arg1)
        {
            OnCardSetUp(arg1.PickUp());
        }
    }
}
