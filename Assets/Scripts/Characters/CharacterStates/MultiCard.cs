using System;
using General;
using ModestTree.Util;

namespace Characters.CharacterStates
{
    public class MultiCard : CharacterState
    {
        private Action<CardPoolable> _onCardUsed;
        
        public override bool OnCardUsed(CardPoolable card)
        {
            _onCardUsed?.Invoke(card);
            return false;
        }

        public void SetOnCardUsed(Action<CardPoolable> onCardUsed)
        {
            _onCardUsed = onCardUsed;
        }
    }
}