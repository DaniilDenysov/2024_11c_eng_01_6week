using Characters;
using General;
using Managers;
using Selectors;
using System.Collections.Generic;
using Client;
using Distributors;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cards
{
    public class HarmCard : Card
    {
        [SerializeField] private GameObject characterSelectionPanel;
        [SerializeField] private Button characterButtonPrefab;

        public override void OnCardActivation(GameObject inventory)
        {
            if (inventory == null)
            {
                OnCardSetUp(false);
                return;
            }

            DisplayCharacterSelection(NetworkPlayerContainer.Instance.GetItems());
        }

        private void DisplayCharacterSelection(List<NetworkPlayer> availableTargets)
        {
            characterSelectionPanel.SetActive(true);

            foreach (var target in availableTargets)
            {
                Button newButton = Instantiate(characterButtonPrefab);
                newButton.GetComponentInChildren<TMP_Text>().text = target.name;
                newButton.gameObject.transform.SetParent(characterSelectionPanel.transform);

                newButton.onClick.AddListener(() => OnPlayerChosen(target));
            }
        }

        private void OnPlayerChosen(NetworkPlayer selectedCharacter)
        {
            characterSelectionPanel.SetActive(false);

            foreach (Transform child in characterSelectionPanel.transform)
            {
                Destroy(child.gameObject);
            }

            if (selectedCharacter.TryGetComponent(out ClientData data))
            {
                data.CmdChangeHarmAmount(true);
                OnCardSetUp(true);
            }
            
            OnCardSetUp(false);
        }
    }
}
