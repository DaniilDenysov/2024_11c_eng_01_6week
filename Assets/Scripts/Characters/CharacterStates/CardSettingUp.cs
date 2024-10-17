using Cards;
using General;

namespace Characters.CharacterStates
{
    public class CardSettingUp : CharacterState
    {
        public override bool IsCardUsable(Card card)
        {
            return false;
        }
        
        public override bool IsMovable()
        {
            return false;
        }
    }
}