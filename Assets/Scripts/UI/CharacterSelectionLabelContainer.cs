using Lobby;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class CharacterSelectionLabelContainer : LabelContainer<CharacterSelectionLabel,CharacterSelectionLabelContainer>
    {
        public void DeselectAllCharacters ()
        {
            foreach (var selector in GetItems())
            {
                selector.SetBlock(false);
            }
        }

        public override CharacterSelectionLabelContainer GetInstance()
        {
            return this;
        }
    }
}
