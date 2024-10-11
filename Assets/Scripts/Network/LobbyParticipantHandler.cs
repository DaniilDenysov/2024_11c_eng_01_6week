using Managers;
using TMPro;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyParticipantHandler : NetworkBehaviour
{
    [SerializeField] private TMP_Text displayName;
    [SerializeField] private Image readyImage; 
    [SerializeField, SyncVar(hook = nameof(OnPlayerNameChanged))] private string playerName;
    [SerializeField, SyncVar(hook = nameof(OnPartyOwnerStateChanged))] private bool isPartyOwner = false;
    [SerializeField, SyncVar(hook = nameof(OnReady))] private bool isReady = false;

    public static event Action<bool> OnPartyOwnerChanged;

    private void OnPartyOwnerStateChanged(bool oldState, bool newState)
    {
        if (!isOwned) return;
        OnPartyOwnerChanged?.Invoke(newState);
    }

    public bool GetReady() => isReady;

    private void OnReady (bool oldState, bool newState)
    {
        readyImage.color = newState == true ? Color.green : Color.red;
    }

    private void OnPlayerNameChanged(string oldName, string newName)
    {
        displayName.text = newName;
    }

    public bool GetPartyOwner() => isPartyOwner;

    [ClientRpc]
    public void AddToContainer ()
    {
        ((CustomNetworkManager)NetworkManager.singleton).AddParticipantToContainer(this);
    }

    [Server]
    public void SetPlayerName(string playerName)
    {
        this.playerName = playerName;
    }

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

    [Command]
    public void CmdReady()
    {
        isReady = !isReady;
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
