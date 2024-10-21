using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class ActionBlockerManager : MonoBehaviour
    {
        private HashSet<string> allowedActions = new HashSet<string>();
        private bool allBlocked = false;

        public void AllowAction(string actionName)
        {
            if (!allowedActions.Contains(actionName))
            {
                allowedActions.Add(actionName);
            }

            allBlocked = false;
        }

        public bool IsActionBlocked(string actionname) 
        {
            if (allBlocked) return true;

            return !allowedActions.Contains(actionname);
        }

        public void BlockAllActions()
        {
            allowedActions.Clear();

            allBlocked = true;
        }

        public void UnblockAllActions()
        {
            allowedActions.Clear();

            allBlocked = false;
        }

        private void Start()
        {
            // Testing the blocking system
            Debug.Log("Initial Action Check (Move): " + IsActionBlocked("Move"));  // Expect false

            // Block all actions
            BlockAllActions();

            Debug.Log("After Blocking All Actions (Move): " + IsActionBlocked("Move"));  // Expect true
            Debug.Log("After Blocking All Actions (Attack): " + IsActionBlocked("Attack"));  // Expect true

            // Allow specific action
            AllowAction("Move");
            Debug.Log("After Allowing Move Action (Move): " + IsActionBlocked("Move"));  // Expect false
            Debug.Log("After Allowing Move Action (Attack): " + IsActionBlocked("Attack"));  // Expect true

            // Unblock all actions
            UnblockAllActions();
            Debug.Log("After Unblocking All Actions (Move): " + IsActionBlocked("Move"));  // Expect false
            Debug.Log("After Unblocking All Actions (Attack): " + IsActionBlocked("Attack"));  // Expect false
        }
    }
}
