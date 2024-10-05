using Collectibles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class EatCard : Card<ICollector>
    {
        public override bool OnCardActivation(ICollector arg1)
        {
            return arg1.PickUp();
        }
    }
}
