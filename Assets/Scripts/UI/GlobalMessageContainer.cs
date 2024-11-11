using Messages;
using Mirror;
using CustomTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalMessageContainer : NetworkBehaviour
{
    public static GlobalMessageContainer Instance;
    [SerializeField] private Message message;
    [SerializeField] private Animator animator;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    [ClientRpc]
    public void DisplayMessage (string header, string body)
    {
        message.Construct(body, header);
        message.gameObject.SetActive(true);
    }
}
