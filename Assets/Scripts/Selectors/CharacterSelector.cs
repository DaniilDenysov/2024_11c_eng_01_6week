using Characters;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Selectors
{
    public class CharacterSelector : MonoBehaviour
    {
        public static CharacterSelector Instance { get; private set; }
        public static CharacterMovement CurrentCharacter { get; private set; }
        [SerializeField] private DiceManager diceManager;
        [SerializeField] private List<CharacterMovement> characters;
        private Queue<CharacterMovement> turnOrder;
        public UnityEvent<string> onStepCountChanged;
        
        private void Awake()
        {
            Instance = this;
            
            turnOrder = new Queue<CharacterMovement>(characters);
            EventManager.OnTurnStart += OnTurnStart;
        }

        private void OnTurnStart()
        {
          //  SelectNext();
        }

        public void SelectNext()
        {
            if (turnOrder.TryDequeue(out CharacterMovement characterMovement))
            {
                turnOrder.Enqueue(characterMovement);
                CurrentCharacter = characterMovement;
              //  onStepCountChanged.Invoke(CurrentCharacter.GetSteps().ToString());

             /*   EventManager.OnCharacterMovesIn += (vector3, movement) =>
                {
                    onStepCountChanged.Invoke(CurrentCharacter.GetSteps().ToString());
                };*/
                
                characterMovement.OnTurn();
            }
        }

        public static void FinishTurn()
        {
            EventManager.FireEvent(EventManager.OnTurnEnd);
        }

        public List<CharacterMovement> GetCharacters()
        {
            return characters;
        }
    }
}
