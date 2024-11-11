using CustomTools;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;

namespace Characters.Skills
{
    public class SkillSelector : MonoBehaviour
    {
        [SerializeField] private HUDSelector selectSkill_HUD;
        [SerializeField] private Skill[] skills;
        private String[] _skillNames;
        private Action<bool> _onSetUp;
        private int _chosenSkillIndex = 0;
        
        public void Awake()
        {
            skills = GetComponents<Skill>();
            _skillNames = skills.Select(e => e.GetType().Name).ToArray();
            selectSkill_HUD.SetButtonTexts(_skillNames);
        }

        public void Select(Action<bool> onSetUp)
        {
            selectSkill_HUD.gameObject.SetActive(true);
            _onSetUp = onSetUp;
        }

        public void SelectSkill(int i)
        {
            if (skills[i].IsActivatable())
            {
                _chosenSkillIndex = i;
                skills[i].Activate(_onSetUp);
            }
            else
            {
                _onSetUp.Invoke(false);
            }
        }

        public void DiscardSkill()
        {
            skills[_chosenSkillIndex].OnDiscard();
        }
    }
}
