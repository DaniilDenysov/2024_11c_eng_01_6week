using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Characters.Skills
{
    public abstract class Skill : MonoBehaviour
    {
        public abstract void Activate();
        public abstract bool IsActivatable();
        public void OnActivated() {
            EventManager.OnSkillSetUp(true);
        }
    }
}
