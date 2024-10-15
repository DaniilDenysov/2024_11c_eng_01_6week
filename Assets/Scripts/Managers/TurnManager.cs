using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class TurnManager : Manager
    {
        public override void Start()
        {
            base.Start();
            OnTurnEnd();
            EventManager.OnTurnEnd += OnTurnEnd;
        }

        private void OnTurnEnd()
        {
            EventManager.FireEvent(EventManager.OnTurnStart);
        }


        public override void InstallBindings()
        {
            Container.Bind<TurnManager>().FromInstance(this).AsSingle();
        }
    }
}
