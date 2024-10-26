using UnityEngine;
using System;
using System.Reflection;
using Characters;

namespace Managers
{
    [CreateAssetMenu(fileName = "Event Manager", menuName = "Create event manager")]
    public class EventManager : ScriptableObject
    {
        public static Action OnClientStartTurn, OnClientEndTurn;
        public static Action OnTurnStart, OnTurnEnd;
        public static Action OnTick;
        
        public static Action<Vector3> OnLitTileClick;
        public static Action<bool> OnSkillSetUp;
        public static Action<bool> OnCardSetUp;

        public static Action<Vector3, CharacterMovement> OnCharacterMovesOut;

        public static Action<Vector3, CharacterMovement> OnCharacterMovesIn;

        public static void FireEvent(Action action)
        {
            action?.Invoke();
        }

        public static void FireEvent<T>(Action<T> action, T arg)
        {
            action?.Invoke(arg);
        }

        public static void FireEvent<T, T1>(Action<T, T1> action, T arg, T1 arg1)
        {
            action?.Invoke(arg, arg1);
        }

        public static void FireEvent(string eventName)
        {
            var field = typeof(EventManager).GetField(eventName, BindingFlags.Public | BindingFlags.Static);

            if (field != null)
            {
                var action = field.GetValue(null) as Action;

                if (action != null)
                {
                    FireEvent(action);
                }
                else
                {
                    Debug.LogError("Event not found or is not a valid delegate: " + eventName);
                }
            }
            else
            {
                Debug.LogError("Event not found: " + eventName);
            }
        }
    }
}
