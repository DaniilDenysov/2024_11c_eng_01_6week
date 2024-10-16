using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class PlayerLabelsContainer : LabelContainer<PlayerLabel, PlayerLabelsContainer>
    {
        public override PlayerLabelsContainer GetInstance()
        {
            return this;
        }
    }
}
