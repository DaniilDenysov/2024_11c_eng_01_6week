using Managers;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyLabel : MonoBehaviour
{
    [SerializeField] private TMP_Text labelName, lobbyInfo;
    [SerializeField] private Button joinButton;
    private CSteamID steamID;

    public void Construct(CSteamID lobbyID, string lobbyName)
    {
        labelName.text = lobbyName;
        steamID = lobbyID;
        int playerCount = SteamMatchmaking.GetNumLobbyMembers(lobbyID);
        int maxPlayerCount = SteamMatchmaking.GetLobbyMemberLimit(lobbyID);
        lobbyInfo.text = $"{playerCount}/4";
        joinButton.interactable = playerCount < 4 ? true : false; 
    }

    public void JoinLobby ()
    {
        if (steamID != null)
        {
            LobbyManager.Instance.JoinLobby(steamID);
        }
    }
}
