using Cards;
using General;
using UnityEngine;

namespace Characters.CharacterStates
{
    public class CardSettingUp : CharacterState
    {
        private Card _card;

        public CardSettingUp(Card card)
        {
            _card = card;
        }
        
        public Card GetCard()
        {
            return _card;
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
    }
}