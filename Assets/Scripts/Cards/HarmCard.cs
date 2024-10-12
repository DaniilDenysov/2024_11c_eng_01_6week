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

        public override void OnCardActivation(Inventory inventory)
        {
            // Check if the current character has an inventory
            if (inventory == null)
            {
                OnCardSetUp(false);
                return;
            }

            // Create a list of available targets (including CurrentCharacter as a single target)
            List<CharacterMovement> availableTargets = new List<CharacterMovement> { CharacterSelector.CurrentCharacter };

            // Call DisplayCharacterSelection from the CharacterSelector
            CharacterSelector.Instance.DisplayCharacterSelection(availableTargets, OnPlayerChosen);
        }

        private void OnPlayerChosen(CharacterMovement selectedCharacter)
        {
            // Assign the target inventory
            targetInventory = selectedCharacter.GetComponent<Inventory>();
            if (targetInventory != null)
            {
                targetInventory.AdjustCardDraw(-1);  // Reduce the card draw by 1 for the target player.
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
            if (attackedPlayer == targetInventory.gameObject)
            {
                // Remove the "Harm" effect when the target is attacked
                targetInventory.AdjustCardDraw(1);  // Restore normal card draw.
                EventManager.OnPlayerAttacked -= OnPlayerAttacked;  // Unsubscribe from the event.
                Destroy(gameObject);  // Remove the card from the game.
            }
        }
    }
}
