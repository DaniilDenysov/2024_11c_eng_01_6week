using Managers;
using TMPro;
using Mirror;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DTOs;
using Lobby;
using UI;

public class PlayerLabel : NetworkBehaviour
{
    public static PlayerLabel LocalPlayer;
    [SerializeField] private Sprite iconPlaceHolder;
    [SerializeField] private TMP_Text displayName, readyDisplay;
    [SerializeField] private Image characterSelected;
    [SyncVar(hook = nameof(OnPlayerStateChanged))] public Player Player = new Player();
    [SerializeField] private List<CharacterData> characters = new List<CharacterData>();

    public static event Action<bool> OnPartyOwnerChanged;

    public override void OnStartClient()
    {
        if (isLocalPlayer)
        {
            Debug.Log("Assigned");
            LocalPlayer = this;
        }
    }

    private void Start()
    {
        PlayerLabelsContainer.Instance.Add(this);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        CharacterSelectionLabel.OnDeselected?.Invoke(Player.CharacterGUID);
    }

    #region commands
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
    #endregion
    #region setters
    [ClientRpc]
    public void SetCharacterIcon (string chracterGUID)
    {
        CharacterData characterData = characters.Where((c) => c.CharacterGUID == chracterGUID).FirstOrDefault();
        if (characterData == null) return;
        characterSelected.sprite = characterData.CharacterIcon;
    }

    [Server]
    public void SetPlayerName(string playerName)
    {
        var player = new Player(Player);
        player.Nickname = playerName;
        Player = player;
    }

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
    #endregion
    #region getters
    public bool GetPartyOwner() => Player.IsPartyOwner;
    public string GetPlayerName() => Player.Nickname;
    public bool GetReady() => Player.IsReady;
    #endregion
    #region callbacks
    public void OnPlayerStateChanged (Player oldState, Player newState)
    {
        string suffix = "";
        if (isLocalPlayer) suffix = " (You)";
        displayName.text = newState.Nickname + suffix;
        readyDisplay.text = newState.IsReady == true ? "Ready" : "Not ready";
        readyDisplay.color = newState.IsReady == true ? Color.green : Color.red;
        CharacterData characterData = characters.Where((c) => c.CharacterGUID == newState.CharacterGUID).FirstOrDefault();

        if (characterData != null)
        {
            CharacterSelectionLabel.OnDeselected?.Invoke(oldState.CharacterGUID);
            // bool isLocal = false;
            /* if (oldState == null)
             {
                 isLocal = newState.ConnectionId == connectionToClient.connectionId; newState.ConnectionId == connectionToClient.connectionId
             }*/
            CharacterSelectionLabel.OnSelected?.Invoke(characterData.CharacterGUID, isLocalPlayer);
            characterSelected.sprite = characterData.CharacterIcon;
        }
        else
        {
            CharacterSelectionLabel.OnDeselected?.Invoke(oldState.CharacterGUID);
            characterSelected.sprite = iconPlaceHolder;
        }
        if (!isOwned) return;
        OnPartyOwnerChanged?.Invoke(newState.IsPartyOwner);
    }
    #endregion
    #region rpcs
    [TargetRpc]
    public void OnValidateSelection()
    {
        CharacterSelectionLabelContainer.Instance.DeselectAllCharacters();
    }

    [ClientRpc]
    public void ValidateSelection()
    {
      if (LocalPlayer != null && Player != null && LocalPlayer.connectionToClient != null)  CharacterSelectionLabel.OnSelected?.Invoke(Player.CharacterGUID, Player.ConnectionId == LocalPlayer.connectionToClient.connectionId);
    }

    [ClientRpc]
    public void AddToContainer ()
    {
        PlayerLabelsContainer.Instance.Add(this);
    }
    #endregion
}
