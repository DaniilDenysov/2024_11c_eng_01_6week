using General;

namespace Characters.CharacterStates
{
    public class CardSetingUp : CharacterState
    {
        public override bool OnCardUsed(CardPoolable card)
        {
            return false;
        }
    }
}