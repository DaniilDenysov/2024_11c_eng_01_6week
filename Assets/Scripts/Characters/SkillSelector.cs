using CustomTools;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Characters.Skills
{
    public class SkillSelector : MonoBehaviour
    {
        [SerializeField] private GameObject selectSkill_HUD;
        [SerializeField] private Skill [] skills;
        private Action<bool> _onSetUp;
        
        public void Awake()
        {
            skills = GetComponents<Skill>();
        }

        public void Select (Action<bool> onSetUp)
        {
            selectSkill_HUD.SetActive(true);
            _onSetUp = onSetUp;
        }

        public void SelectSkill(int i)
        {
            if (skills[i].IsActivatable())
            {
                skills[i].Activate(_onSetUp);
            }
            else
            {
                _onSetUp.Invoke(false);
            }
        }
    }
}
