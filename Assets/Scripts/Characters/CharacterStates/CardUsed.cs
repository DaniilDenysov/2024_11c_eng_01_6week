using System.Collections.Generic;
using System.Numerics;
using Cards;

namespace Characters.CharacterStates
{
    public class  CardUsed : CharacterState
    {
        private bool _isCardTransforms;

        public CardUsed(bool isCardTransforms)
        {
            _isCardTransforms = isCardTransforms;
        }
        
        public override bool IsCardUsable(Card card)
        {
            return _isCardTransforms;
        }
        
        public override bool IsMovable()
        {
            return true;
        }
    }
}