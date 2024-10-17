using System.Collections.Generic;
using System.Numerics;
using Cards;

namespace Characters.CharacterStates
{
    public class CardUsed : CharacterState
    {
        public override bool IsCardUsable(Card card)
        {
            return false;
        }
        
        public override bool IsMovable()
        {
            return true;
        }
    }
}