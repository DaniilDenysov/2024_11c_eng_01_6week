using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using Mirror;
using UnityEngine;

namespace Characters.Skills
{
    public abstract class Skill : NetworkBehaviour
    {
        private Action<bool> _onSetUp;

        public virtual void Activate(Action<bool> onSetUp)
        {
            _onSetUp = onSetUp;
        }
        public abstract bool IsActivatable();
        
        public void OnActivated(bool isSuccessful = true) {
            _onSetUp.Invoke(isSuccessful);
        }
    }
}
