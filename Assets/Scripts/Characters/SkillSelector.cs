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
        
        public void Awake()
        {
            skills = GetComponents<Skill>();
        }


        public void Select ()
        {
            selectSkill_HUD.SetActive(true);
        }

        public void SelectSkill(int i)
        {
            if (skills[i].IsActivatable())
            {
                skills[i].Activate();
            }
            else
            {
                EventManager.OnSkillSetUp(false);
            }
        }
    }
}
