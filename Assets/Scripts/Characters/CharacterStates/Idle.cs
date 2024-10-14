using General;
using UnityEngine;

namespace Characters.CharacterStates
{
    public class Idle : CharacterState
    {
        public override bool OnCardUsed(CardPoolable card)
        {
            return true;
        }
    }
}