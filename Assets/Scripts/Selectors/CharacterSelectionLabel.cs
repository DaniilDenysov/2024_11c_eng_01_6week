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
        public static Action<string,bool> OnSelected;
        public static Action<string> OnDeselected;
        [SerializeField] private Image icon;
        [SerializeField] private CharacterData characterData;
        [SerializeField] private GameObject selectButton, cancelButton;

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

        public void OnCharacterSelected (string character, bool isLocalPlayer)
        {
            if (characterData.CharacterGUID == character)
            {
                selectButton.SetActive(false);
                
                if (isLocalPlayer)
                {
                    cancelButton.SetActive(true);
                }
                else
                {
                    icon.color = Color.gray;
                }
            }
        }

        public void OnCharacterDeselected(string character)
        {
            if (characterData.CharacterGUID == character)
            {
                icon.color = Color.white;
                selectButton.SetActive(true);
                cancelButton.SetActive(false);
            }
        }


        public void SelectCharacter ()
        {
           PlayerLabel.LocalPlayer.SetCharacterGUID(characterData.CharacterGUID);
        }

        public void Deselect ()
        {
            PlayerLabel.LocalPlayer.SetCharacterGUID("");
        }

        public void SetBlock (bool state)
        {
            selectButton.SetActive(state);
        }
    }
}
