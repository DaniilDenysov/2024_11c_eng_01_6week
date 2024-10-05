using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cards
{
    public class HarmCard : Card<TilemapPath>
    {
        public override bool OnCardActivation(TilemapPath arg1)
        {
            return true;
        }
    }
}
