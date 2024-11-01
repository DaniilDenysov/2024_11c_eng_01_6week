using System;
using DTOs;
using Mirror;
using TMPro;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{

    public static NetworkPlayer LocalPlayerInstance;

    [SerializeField,SyncVar(hook = nameof(OnPlayerDataChanged))] private Player player;

    [SerializeField] private CharacterData characterData;
    [SerializeField] private TMP_Text displayName; 

    public string GetCharacterGUID() => characterData.CharacterGUID;
    public Player GetPlayerData() =>player;

    private void OnPlayerDataChanged(Player oldValue, Player newValue)
    {
      /*  string name = PlayerPrefs.GetString("Nickname");
        if (string.IsNullOrEmpty(name) && name != player.Nickname)
        {
            name = $"Player{newValue.ConnectionId}";
        }
        else
        {
            player.Nickname = name;
        }*/
        displayName.text = newValue.Nickname;
    }

    public override void OnStartAuthority()
    {
        if (isOwned)
        {
            LocalPlayerInstance = this;
        }
    }

    public override void OnStopAuthority()
    {
        if (isOwned)
        {
            LocalPlayerInstance = null;
        }
    }

    public CharacterData GetCharacterData() => characterData;

    [Server]
    public void SetPlayer(Player player)
    {
        this.player = player;
    }
}
