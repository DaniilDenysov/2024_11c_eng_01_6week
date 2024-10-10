using Managers;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyParticipantHandler : NetworkBehaviour
{
    [SerializeField, SyncVar(hook = nameof(OnPartyOwnerStateChanged))] private bool isPartyOwner = false;

    public static event Action<bool> OnPartyOwnerChanged;

    private void OnPartyOwnerStateChanged(bool oldState, bool newState)
    {
        if (!isOwned) return;
        OnPartyOwnerChanged?.Invoke(newState);
    }

    public bool GetPartyOwner() => isPartyOwner;

    [Server]
    public void SetPartyOwner (bool isPartyOwner)
    {
        this.isPartyOwner = isPartyOwner;
    }

    [Command]
    public void CmdStartGame ()
    {
        if (!isPartyOwner) return;

        ((CustomNetworkManager)NetworkManager.singleton).StartGame();
    }

    private void OnEnable()
    {
        CustomNetworkManager.OnClientConnected += OnClientConnected;
        CustomNetworkManager.OnClientDisconnected += OnClientDisconnected;
    }

    private void OnDisable()
    {
        CustomNetworkManager.OnClientConnected -= OnClientConnected;
        CustomNetworkManager.OnClientDisconnected -= OnClientDisconnected;
    }

    private void OnClientDisconnected()
    {

    }

    private void OnClientConnected()
    {

    }

}
