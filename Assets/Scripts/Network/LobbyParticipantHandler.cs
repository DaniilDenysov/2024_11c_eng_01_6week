using Managers;
using TMPro;
using Mirror;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using UnityEngine.UI;
using DTOs;
using Lobby;

public class LobbyParticipantHandler : NetworkBehaviour
{
    [SerializeField] private TMP_Text displayName;
    [SerializeField] private Image readyImage, characterSelected;
 //   [SerializeField, SyncVar(hook = nameof(OnPlayerNameChanged))] private string playerName;
 //   [SerializeField, SyncVar(hook = nameof(OnPartyOwnerStateChanged))] private bool isPartyOwner = false;
  //  [SerializeField, SyncVar(hook = nameof(OnReady))] private bool isReady = false;
    [SyncVar(hook = nameof(OnPlayerStateChanged))] public Player Player = new Player();
    [SerializeField] private List<CharacterData> characters = new List<CharacterData>();

    public static event Action<bool> OnPartyOwnerChanged;


    public void Awake()
    {
        CustomNetworkManager.OnValidateStates += OnValidateState;
    }

    [ClientRpc]
    public void DeselectOnClient ()
    {
        LobbyManager.Instance.DeselectAllCharacters();
    }

    [ClientRpc]
    public void OnValidateState()
    {
        Debug.Log("Validated");
        LobbyCharacterSelector.OnSelected?.Invoke(Player.CharacterGUID);
    }

    public void OnPlayerStateChanged (Player oldState, Player newState)
    {
        Debug.Log("Changed");
        displayName.text = newState.Nickname;
        readyImage.color = newState.IsReady == true ? Color.green : Color.red;
        CharacterData characterData = characters.Where((c) => c.CharacterGUID == newState.CharacterGUID).FirstOrDefault();

        if (characterData != null)
        {
            LobbyCharacterSelector.OnDeselected?.Invoke(oldState.CharacterGUID);
            LobbyCharacterSelector.OnSelected?.Invoke(characterData.CharacterGUID);
            characterSelected.sprite = characterData.CharacterIcon;
        }
        if (!isOwned) return;
        OnPartyOwnerChanged?.Invoke(newState.IsPartyOwner);
    }

    [ClientRpc]
    public void UpdateClient(List<int> connectionIds)
    {

        ((CustomNetworkManager)NetworkManager.singleton).UpdateSelection(connectionIds);
    }

   

    public bool GetReady() => Player.IsReady;

    [ClientRpc]
    public void SetCharacterIcon (string chracterGUID)
    {
        CharacterData characterData = characters.Where((c) => c.CharacterGUID == chracterGUID).FirstOrDefault();
        if (characterData == null) return;
        characterSelected.sprite = characterData.CharacterIcon;
    }


    public bool GetPartyOwner() => Player.IsPartyOwner;

    [ClientRpc]
    public void AddToContainer ()
    {
        ((CustomNetworkManager)NetworkManager.singleton).AddParticipantToContainer(this);
    }

    [Server]
    public void SetPlayerName(string playerName)
    {
        var player = new Player(Player);
        player.Nickname = playerName;
        Player = player;
    }

    public string GetPlayerName() => Player.Nickname; 

    [Server]
    public void SetPartyOwner (bool isPartyOwner)
    {
        var player = new Player(Player);
        player.IsPartyOwner = isPartyOwner;
        Player = player;
    }

    [Server]
    public void SetConnectionId(int connectionId)
    {
        var player = new Player(Player);
        player.ConnectionId = connectionId;
        Player = player;
    }

    [Command]
    public void SetCharacterGUID(string characterGUID)
    {
        var player = new Player(Player);
        player.CharacterGUID = characterGUID;
        Player = player;
    }

    [Command]
    public void CmdStartGame ()
    {
        if (!Player.IsPartyOwner) return;

        ((CustomNetworkManager)NetworkManager.singleton).StartGame();
    }


 
    [Command]
    public void CmdReady()
    {
        var player = new Player(Player);
        player.IsReady = !Player.IsReady;
        Player = player;
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
        CustomNetworkManager.OnValidateStates -= OnValidateState;
    }

    private void OnClientConnected()
    {

    }

}
