using Cards;
using General;

namespace Characters.CharacterStates
{
    public class CardSettingUp : CharacterState
    {
        public override bool OnCardUsed(Card card)
        {
            return false;
        }
    }
}