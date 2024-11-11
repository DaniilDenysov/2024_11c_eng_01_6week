using Cards;
using General;
using UnityEngine;

namespace Characters.CharacterStates
{
    public class Idle : CharacterState
    {
        public override bool IsCardUsable(Card card)
        {
            return true;
        }
        
        public override bool IsMovable()
        {
            return true;
        }
        
        public override bool IsCardDiscardable(Card card)
        {
            return false;
        }
    }
}