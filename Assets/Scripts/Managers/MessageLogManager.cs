using Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class MessageLogManager : Manager
    {
        [SerializeField] private Transform messageContainer;
        [SerializeField] private Message messageWindowPrefab, messagePrefab;


        public void DisplayMessage (string body)
        {
            Instantiate(messageWindowPrefab.Construct(body)).transform.SetParent(messageContainer);
        }

        public void DisplayMessage(string body, string header)
        {
            Instantiate(messageWindowPrefab.Construct(body)).transform.SetParent(messageContainer);
        }
    }
}
