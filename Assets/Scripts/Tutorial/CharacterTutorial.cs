using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using General;

namespace Tutorial
{
    public class CharacterTutorial : MonoBehaviour
    {

        [SerializeField] private SerializedDictionary<CharacterData, GameObject> characters = new SerializedDictionary<CharacterData, GameObject>();
        private CharacterData currentCharacter;

        private void OnEnable()
        {
            if (NetworkPlayer.LocalPlayerInstance == null) return;
            currentCharacter = NetworkPlayer.LocalPlayerInstance.GetCharacterData();
            if (characters.TryGetValue(currentCharacter, out GameObject window))
            {
                window.SetActive(true);
            }
        }

        private void OnDisable()
        {
            if (characters.TryGetValue(currentCharacter, out GameObject window))
            {
                window.SetActive(false);
            }
        }
    }
}
