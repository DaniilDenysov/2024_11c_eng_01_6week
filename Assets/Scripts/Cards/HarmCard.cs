using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cards
{
    public class HarmCard : Card
    {
        public override void OnCardActivation(GameObject activator)
        {
            OnCardSetUp(true);
        }
    }
}
