using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using Steamworks;
using System.Linq;

public class LobbyLabelsContainer : LabelContainer<LobbyLabel, LobbyLabelsContainer>
{
    [SerializeField] private LobbyLabel lobbyLabel;
    private Callback<LobbyMatchList_t> lobbyListCallback;
    private Callback<LobbyDataUpdate_t> lobbyDataUpdatedCallback;


    public override LobbyLabelsContainer GetInstance()
    {
        return this;
    }

    private void Start()
    {
        if (SteamManager.Initialized)
        {
            lobbyListCallback = Callback<LobbyMatchList_t>.Create(OnLobbyListReceived);
            lobbyDataUpdatedCallback = Callback<LobbyDataUpdate_t>.Create(OnLobbyDataUpdated);

           // RequestLobbiesForGame();
        }
        else
        {
            Debug.LogError("Steam is not initialized.");
        }
    }

    public void RequestLobbiesForGame()
    {
        // Add a filter to only find lobbies with a specific game key (e.g., "game_id" or "game_name")
        ClearList();
        SteamMatchmaking.AddRequestLobbyListStringFilter("game_id", "Labyrism", ELobbyComparison.k_ELobbyComparisonEqual);

        // Request the list of lobbies
        SteamMatchmaking.RequestLobbyList();
    }

    private void OnLobbyListReceived(LobbyMatchList_t result)
    {
        Debug.Log($"Number of lobbies found: {result.m_nLobbiesMatching}");

        for (int i = 0; i < result.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);

            // Request lobby data for each lobby found
            SteamMatchmaking.RequestLobbyData(lobbyID);
        }
    }

    private void OnLobbyDataUpdated(LobbyDataUpdate_t lobbyData)
    {
        ClearList();
        if (lobbyData.m_bSuccess != 1 || !lobbyData.m_ulSteamIDLobby.Equals(lobbyData.m_ulSteamIDMember))
            return;

        CSteamID lobbyID = new CSteamID(lobbyData.m_ulSteamIDLobby);
        string lobbyName = SteamMatchmaking.GetLobbyData(lobbyID, "name");
        string gameID = SteamMatchmaking.GetLobbyData(lobbyID, "game_id");

        Debug.Log($"Lobby found: {lobbyName} (ID: {lobbyID})");
        if (gameID == "Labyrism")
        {
 
            LobbyLabel label = Instantiate(lobbyLabel);
            Add(label);
            label.Construct(lobbyID,lobbyName);
            if (label.TryGetComponent(out RectTransform rect))
            {
                rect.localScale = new Vector3(1f, 1f);
            }
        }
    }

    private void ClearList ()
    {
        foreach (var item in items)
        {
            if (item != null) Destroy(item.gameObject);
        }
        items.Clear();
    }
}
