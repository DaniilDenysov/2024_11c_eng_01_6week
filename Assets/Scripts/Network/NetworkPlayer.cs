using System;
using DTOs;
using Mirror;
using TMPro;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
   
    [SerializeField,SyncVar(hook = nameof(OnPlayerDataChanged))] private Player player;

    [SerializeField] private CharacterData characterData;
    [SerializeField] private TMP_Text displayName; 

    public string GetCharacterGUID() => characterData.CharacterGUID;
    public Player GetPlayerData() =>player;

    private void OnPlayerDataChanged(Player oldValue, Player newValue)
    {
        string name = PlayerPrefs.GetString("Nickname");
        if (string.IsNullOrEmpty(name) && name != player.Nickname)
        {
            name = $"Player{newValue.ConnectionId}";
        }
        else
        {
            player.Nickname = name;
        }
        displayName.text = player.Nickname;
    }

    [Server]
    public void SetPlayer(Player player)
    {
        this.player = player;
    }
}
