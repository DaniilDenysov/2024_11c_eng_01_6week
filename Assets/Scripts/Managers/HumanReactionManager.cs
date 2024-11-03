using System.Collections.Generic;
using UnityEngine;
using Characters;
using Collectibles;

namespace Managers
{
    public class HumanReactionManager : MonoBehaviour
    {
        [SerializeField] private float fearRange = 5f;
        private List<Human> humans = new List<Human>();

        private void Awake()
        {
            EventManager.OnCharacterMovesIn += OnCharacterMovesIn;
            EventManager.OnCharacterMovesOut += OnCharacterMovesOut;
        }

        private void OnDestroy()
        {
            EventManager.OnCharacterMovesIn -= OnCharacterMovesIn;
            EventManager.OnCharacterMovesOut -= OnCharacterMovesOut;
        }

        public void RegisterHuman(Human human)
        {
            if (!humans.Contains(human))
            {
                humans.Add(human);
            }
        }

        public void UnregisterHuman(Human human)
        {
            if (humans.Contains(human))
            {
                humans.Remove(human);
            }
        }

        private void OnCharacterMovesIn(Vector3 position, CharacterMovement character)
        {
            foreach (var human in humans)
            {
                float distance = Vector3.Distance(human.transform.position, position);
                if (distance <= fearRange)
                {
                    human.EnterPanic();
                }
            }
        }

        private void OnCharacterMovesOut(Vector3 position, CharacterMovement character)
        {
            foreach (var human in humans)
            {
                float distance = Vector3.Distance(human.transform.position, position);
                if (distance > fearRange)
                {
                    human.ExitPanic();
                }
            }
        }
    }
}
