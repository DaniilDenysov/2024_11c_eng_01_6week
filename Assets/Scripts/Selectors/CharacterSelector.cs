using Characters;
using Managers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Selectors
{
    public class CharacterSelector : MonoBehaviour
    {
        public static CharacterSelector Instance { get; private set; }
        public static CharacterMovement CurrentCharacter { get; private set; }
        [SerializeField] private DiceManager diceManager;
        [SerializeField] public List<CharacterMovement> characters = new List<CharacterMovement>();
        private Queue<CharacterMovement> turnOrder;
        public UnityEvent<string> onStepCountChanged;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            turnOrder = new Queue<CharacterMovement>(characters);
            EventManager.OnTurnStart += OnTurnStart;
        }

        public void RegisterCharacter(CharacterMovement character)
        {
            if (!characters.Contains(character))
            {
                characters.Add(character);
            }
        }

        private void OnTurnStart()
        {
            SelectNext();
        }

        public void SelectNext()
        {
            if (turnOrder.TryDequeue(out CharacterMovement characterMovement))
            {
                turnOrder.Enqueue(characterMovement);
                CurrentCharacter = characterMovement;
                CurrentCharacter.SetSteps(diceManager.GetDiceValue());
                onStepCountChanged.Invoke(CurrentCharacter.GetSteps().ToString());
                characterMovement.ChooseNewDirection(() => { });
            }
        }

        public static void FinishTurn()
        {
            EventManager.FireEvent(EventManager.OnTurnEnd);
        }

        public void MakeMovement()
        {
            CurrentCharacter.MakeMovement();
            onStepCountChanged.Invoke(CurrentCharacter.GetSteps().ToString());
        }

        public void DisplayCharacterSelection(List<CharacterMovement> availableTargets, Action<CharacterMovement> onTargetSelected)
        {
            foreach (var target in availableTargets)
            {
                Debug.Log($"Available Target: {target.name}");
            }

            if (availableTargets.Count > 0)
            {
                onTargetSelected?.Invoke(availableTargets[0]);
            }
        }
    }
}
