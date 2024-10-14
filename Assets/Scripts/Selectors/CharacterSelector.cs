using Characters;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Selectors
{
    public class CharacterSelector : MonoBehaviour
    {
        public static CharacterSelector Instance { get; private set; }
        public static CharacterMovement CurrentCharacter { get; private set; }
        [SerializeField] private DiceManager diceManager; //inject using zenject later
        [SerializeField] public List<CharacterMovement> characters = new List<CharacterMovement>();
        private Queue<CharacterMovement> turnOrder;
        public List<CharacterMovement> Characters { get; private set; } = new List<CharacterMovement>();


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

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }


        public void SelectNext ()
        {
            if (turnOrder.TryDequeue(out CharacterMovement characterMovement))
            {
                turnOrder.Enqueue(characterMovement);
                CurrentCharacter = characterMovement;
                CurrentCharacter.AddSteps(diceManager.GetDiceValue());
            }
        }

        public void MakeMovement() {
            CurrentCharacter.MakeMovement();
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

    public static class ListExtensions
    {
        public static void Shuffle<T>(this List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                T temp = list[i];
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }
    }

}
