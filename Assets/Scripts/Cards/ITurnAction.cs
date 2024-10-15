using System;
using System.Collections;
using System.Collections.Generic;
using ModestTree.Util;
using UnityEngine;

namespace Cards
{
    public interface ITurnAction
    {
        void OnTurn();
        void ChooseNewDirection(Action onDirectionChosen);
    }
}
