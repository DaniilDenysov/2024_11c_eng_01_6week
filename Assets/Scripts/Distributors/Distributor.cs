using Managers;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Distributors
{
    public abstract class Distributor : NetworkBehaviour
    {
        public override void OnStartClient()
        {
            if (!isServer) return;
            EventManager.OnTurnStart += OnTurnStart;
            EventManager.OnTurnEnd += OnTurnEnd;
        }

        public override void OnStopClient()
        {   
            if (!isServer) return;
            EventManager.OnTurnStart -= OnTurnStart;
            EventManager.OnTurnEnd -= OnTurnEnd;
        }

        public virtual void OnTurnStart()
        {

        }

        public virtual void OnTurnEnd()
        {

        }

    }
}
