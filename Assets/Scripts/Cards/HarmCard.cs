using Characters;
using Managers;
using Selectors;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cards
{
    public class HarmCard : Card
    {
        [SerializeField] private GameObject characterSelectionPanel;
        [SerializeField] private Button characterButtonPrefab;
        private Inventory targetInventory;
        private bool harmActive;

        public override void OnCardActivation(GameObject inventory)
        {
            if (inventory == null)
            {
                OnCardSetUp(false);
                return;
            }

            List<CharacterMovement> availableTargets = new List<CharacterMovement>(CharacterSelector.Instance.characters);
            DisplayCharacterSelection(availableTargets);
        }

        private void DisplayCharacterSelection(List<CharacterMovement> availableTargets)
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

        private void OnPlayerChosen(CharacterMovement selectedCharacter)
        {
            characterSelectionPanel.SetActive(false);

            foreach (Transform child in characterSelectionPanel.transform)
            {
                Destroy(child.gameObject);
            }

            targetInventory = selectedCharacter.GetComponent<Inventory>();
            if (targetInventory != null)
            {
                harmActive = true;
                targetInventory.AdjustCardDraw(-1);
                EventManager.OnPlayerAttacked += OnPlayerAttacked;
                OnCardSetUp(true);
            }
            else
            {
                OnCardSetUp(false);
            }
        }

        private void OnPlayerAttacked(GameObject attackedPlayer)
        {
            if (attackedPlayer == targetInventory.gameObject && harmActive)
            {
                targetInventory.AdjustCardDraw(1);
                EventManager.OnPlayerAttacked -= OnPlayerAttacked;
                harmActive = false;
                Destroy(gameObject);
            }
        }
    }
}
