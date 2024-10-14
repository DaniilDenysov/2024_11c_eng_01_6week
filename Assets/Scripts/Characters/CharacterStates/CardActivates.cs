using General;

namespace Characters.CharacterStates
{
    public class CardActivates : CharacterState
    {
        public override bool OnCardUsed(CardPoolable card)
        {
            return false;
        }
    }
}