using Cards;
using General;
using UnityEngine;

namespace Characters.CharacterStates
{
    public class Idle : CharacterState
    {
        public override bool OnCardUsed(Card card)
        {
            return true;
        }
    }
}