using Managers;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class CharacterSelectionLabel : MonoBehaviour
    {        
        public static Action<string> OnDeselected, OnSelected;
        [SerializeField] private CharacterData characterData;
        [SerializeField] private GameObject blocker;

        public CharacterData GetCharacterData() => characterData;

        public void OnEnable()
        {
            OnDeselected += OnCharacterDeselected;
            OnSelected += OnCharacterSelected;
        }

        public void OnDisable()
        {
            OnDeselected -= OnCharacterDeselected;
            OnSelected -= OnCharacterSelected;
        }

        public void OnCharacterSelected (string character)
        {
            if (characterData.CharacterGUID == character)
            {
                blocker.SetActive(true);
            }
        }

        public void OnCharacterDeselected(string character)
        {
            if (characterData.CharacterGUID == character)
            {
                blocker.SetActive(false);
            }
        }


        public void SelectCharacter ()
        {
           PlayerLabel.LocalPlayer.SetCharacterGUID(characterData.CharacterGUID);
        }

       
        public void SetBlock (bool state)
        {
            blocker.SetActive(state);
        }
    }
}
