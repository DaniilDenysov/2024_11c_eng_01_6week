using System;
using Cards;
using General;
using ModestTree.Util;

namespace Characters.CharacterStates
{
    public class MultiCard : CharacterState
    {
        private Action<Card> _onCardUsed;

        public MultiCard(Action<Card> onCardUsed)
        {
            _onCardUsed = onCardUsed;
        }
        
        public override bool IsCardUsable(Card card)
        {
            _onCardUsed?.Invoke(card);
            return false;
        }
        
        public override bool IsMovable()
        {
            return false;
        }
    }
}