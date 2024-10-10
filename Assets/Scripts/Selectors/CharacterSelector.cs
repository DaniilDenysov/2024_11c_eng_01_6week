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
        public static CharacterMovement CurrentCharacter { get; private set; }
        [SerializeField] private DiceManager diceManager; //inject using zenject later
        [SerializeField] private List<CharacterMovement> characters = new List<CharacterMovement>();
        private Queue<CharacterMovement> turnOrder;
        
        
        private void Awake()
        {
            turnOrder = new Queue<CharacterMovement>(characters);
            EventManager.OnTurnStart += OnTurnStart;
        }

        private void OnTurnStart()
        {
            SelectNext();
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
