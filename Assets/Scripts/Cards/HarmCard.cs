using Characters;
using Managers;
using Selectors;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class HarmCard : Card<Inventory>
    {
        private Inventory targetInventory;
        private bool harmActive;

        public override void OnCardActivation(Inventory inventory)
        {
            if (inventory == null)
            {
                OnCardSetUp(false);
                return;
            }

            List<CharacterMovement> availableTargets = new List<CharacterMovement> { CharacterSelector.CurrentCharacter };

            // Call DisplayCharacterSelection from the CharacterSelector
            CharacterSelector.Instance.DisplayCharacterSelection(availableTargets, OnPlayerChosen);
        }

        private void OnPlayerChosen(CharacterMovement selectedCharacter)
        {
            targetInventory = selectedCharacter.GetComponent<Inventory>();
            if (targetInventory != null)
            {
                harmActive = true;
                targetInventory.AdjustCardDraw(-1);
                EventManager.OnPlayerAttacked += OnPlayerAttacked;  // Subscribe to event when target is attacked.
                OnCardSetUp(true);  // Confirm card setup.
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
                if (targetInventory.TryPopItem(out Human human))
                {
                    Debug.Log("Harm active: Preventing human loss.");
                }

                targetInventory.AdjustCardDraw(1);
                EventManager.OnPlayerAttacked -= OnPlayerAttacked;
                harmActive = false;
                Destroy(gameObject);
            }
        }
    }
}
