using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cards
{
    public class HarmCard : Card<TilemapPath>
    {
        public override void OnCardActivation(TilemapPath arg1)
        {
            OnCardSetUp(true);
        }
    }
}
