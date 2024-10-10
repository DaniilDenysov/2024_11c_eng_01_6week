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
        
        [PreCompilationConstructor]
        public void RefreshSkills ()
        {
            skills = GetComponents<Skill>();
        }


        public void Select ()
        {
            selectSkill_HUD.SetActive(true);
        }

        public void SelectSkill (int i)
        {
            try
            {
                if (skills[i].IsActivatable()) {
                    skills[i].Activate();
                } else {
                    EventManager.OnSkillSetUp(false);
                }
            }
            catch (Exception)
            {
                Debug.LogError($"Out of bounds:unable to find skill with index {i}!");
            }
        }
    }
}
