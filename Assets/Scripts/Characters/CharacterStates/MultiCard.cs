using System;
using Cards;
using General;
using ModestTree.Util;

namespace Characters.CharacterStates
{
    public class MultiCard : CharacterState
    {
        private Action<Card> _onCardUsed;
        private Card _card;

        public MultiCard(Action<Card> onCardUsed, Card card)
        {
            _onCardUsed = onCardUsed;
            _card = card;
        }
        
        public override bool IsCardUsable(Card card)
        {
            return false;
        }
        
        public override bool IsMovable()
        {
            return false;
        }
        
        public override bool IsCardDiscardable(Card card)
        {
            return _card == card;
        }

        public void OnCardActivation(Card card)
        {
            _onCardUsed?.Invoke(card);
        }
    }
}