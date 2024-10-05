using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class TurnManager : Manager
    {
        private void Start()
        {
            OnTurnEnd();
            EventManager.OnTurnEnd += OnTurnEnd;
        }

        private void OnTurnEnd()
        {
            EventManager.FireEvent(EventManager.OnTurnStart);
        }
    }
}
