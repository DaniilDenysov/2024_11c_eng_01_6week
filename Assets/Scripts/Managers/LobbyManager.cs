using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System;
using Lobby;
using System.Threading.Tasks;
using UI;
using DesignPatterns.Singleton;
using Steamworks;

namespace Managers
{
    public class LobbyManager : Singleton<LobbyManager>
    {
        [SerializeField] private GameObject startGameButton;
        [SerializeField] private GameObject loadingLobbyScreen;
        [SerializeField] private GameObject lobbyScreen;
        [SerializeField] private GameObject searchingLobbyScreen;
        [SerializeField] private string lobbyName, hostAddress;
        [SerializeField] private UnityEvent onConnectedToLobby,onDisconnectedFromLobby;

        private Callback<LobbyCreated_t> lobbyCreated;
        private Callback<GameLobbyJoinRequested_t> joinLobbyRequested;
        private Callback<LobbyEnter_t> lobbyEntered;

    /*    private Callback<LobbyMatchList_t> lobbyMatchListCallback;
        private bool lobbySearchInProgress = false;
        private CSteamID firstLobbyFound;*/

        private void Start()
        {
            CustomNetworkManager.OnClientConnected += OnClientConnected;
            CustomNetworkManager.OnClientDisconnected += OnClientDisconnected;
            PlayerLabel.OnPartyOwnerChanged += OnPartyOwnerChanged;
            if (SteamManager.Initialized)
            {
                lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
                joinLobbyRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
                lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
                loadingLobbyScreen.SetActive(false);
            }
            else
            {
                loadingLobbyScreen.SetActive(true);
            }
        }


     /*   public bool TryGetFirstAvailableLobby(out CSteamID cSteamID)
        {
            cSteamID = default;

            if (SteamManager.Initialized && !lobbySearchInProgress)
            {
                lobbySearchInProgress = true;
                lobbyMatchListCallback = Callback<LobbyMatchList_t>.Create(OnLobbyMatchList);
                SteamMatchmaking.AddRequestLobbyListStringFilter("game_id", "Labyrism", ELobbyComparison.k_ELobbyComparisonEqual);
                SteamMatchmaking.RequestLobbyList();
                return true; // Request initiated, will return result asynchronously
            }
            return false; // Return false if Steam is not initialized or a search is already in progress
        }

        private void OnLobbyMatchList(LobbyMatchList_t result)
        {
            lobbySearchInProgress = false;
            if (result.m_nLobbiesMatching > 0)
            {
                // Get the first available lobby's ID
                firstLobbyFound = SteamMatchmaking.GetLobbyByIndex(0);
                Debug.Log("First available lobby found: " + firstLobbyFound);
            }
            else
            {
                firstLobbyFound = default; // No lobby found
                Debug.Log("No available lobbies found.");
            }
        }

        public IEnumerator LookForLobbies()
        {
            loadingLobbyScreen.SetActive(true);
            CSteamID lobbyID;
            while (true)
            {
                // Attempt to start looking for the first available lobby
                if (TryGetFirstAvailableLobby(out lobbyID))
                {
                    // Wait until the callback assigns a valid CSteamID or confirms no lobbies were found
                    while (lobbySearchInProgress)
                    {
                        yield return null; // Wait for the callback to finish
                    }

                    // Check if a valid lobby ID was found
                    if (firstLobbyFound != default)
                    {
                        loadingLobbyScreen.SetActive(false);
                        JoinLobby(firstLobbyFound);
                        yield break; // Exit the coroutine once a lobby is found and joined
                    }
                }

                // Optionally add a delay between retries to avoid hammering the API
                yield return new WaitForSeconds(1f);
            }
        }*/


        #region SteamCallbacks
        private void OnLobbyCreated(LobbyCreated_t param)
        {
            if (param.m_eResult != EResult.k_EResultOK)
            {
                loadingLobbyScreen.SetActive(false);
                return;
            }
     
            var id = new CSteamID(param.m_ulSteamIDLobby);
            SteamMatchmaking.SetLobbyData(id, "name", lobbyName);
            SteamMatchmaking.SetLobbyData(id, "game_id", "Labyrism");
            SteamMatchmaking.SetLobbyData(id, lobbyName, SteamUser.GetSteamID().ToString());
            ((CustomNetworkManager)NetworkManager.singleton).SteamID = id;
            NetworkManager.singleton.StartHost();
        }

        private void OnLobbyEntered(LobbyEnter_t param)
        {
            if (NetworkServer.active) return;
            var id = new CSteamID(param.m_ulSteamIDLobby);

            NetworkManager.singleton.networkAddress = SteamMatchmaking.GetLobbyData(id, SteamMatchmaking.GetLobbyData(id, "name"));
            ((CustomNetworkManager)NetworkManager.singleton).SteamID = id;
            NetworkManager.singleton.StartClient();
        }

        private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t param)
        {
            SteamMatchmaking.JoinLobby(param.m_steamIDLobby);
            loadingLobbyScreen.SetActive(true);
        }
        #endregion


        public void SetLobbyName (string lobbyName)
        {
            this.lobbyName = lobbyName;
        }

        private void OnPartyOwnerChanged(bool obj)
        {
            startGameButton.SetActive(obj);
        }

        public void Ready ()
        {
            NetworkClient.connection.identity.GetComponent<PlayerLabel>().CmdReady();
        }

        private void OnDestroy()
        {
            CustomNetworkManager.OnClientConnected -= OnClientConnected;
            CustomNetworkManager.OnClientDisconnected -= OnClientDisconnected;
            PlayerLabel.OnPartyOwnerChanged -= OnPartyOwnerChanged;
        }

        public void JoinLobby()
        {
            NetworkManager.singleton.networkAddress = lobbyName;
            NetworkManager.singleton.StartClient();
        }

        public void JoinLobby(CSteamID lobbyID)
        {
            if (SteamManager.Initialized)
            {
                loadingLobbyScreen.SetActive(true);
                SteamMatchmaking.JoinLobby(lobbyID);
            }
            else
            {
                Debug.LogError("Steam not initialized. Ensure Steamworks is properly set up.");
            }
        }

        public  void LeaveLobby ()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopHost();

                onDisconnectedFromLobby?.Invoke();
            }
            else
            {
                NetworkManager.singleton.StopClient();

                onDisconnectedFromLobby?.Invoke();
            }
        }

        public void StartGame ()
        {
            NetworkClient.connection.identity.GetComponent<PlayerLabel>().CmdStartGame();
        }

        private void OnClientConnected()
        {
            onConnectedToLobby?.Invoke();
            loadingLobbyScreen.SetActive(false);
        }

        private void OnClientDisconnected()
        {
            CharacterSelectionLabelContainer.Instance.DeselectAllCharacters();
            onDisconnectedFromLobby?.Invoke();
        }

        public void CreateLobby()
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic,4);
            loadingLobbyScreen.SetActive(true);
            lobbyScreen.SetActive(true);
        }

        public override LobbyManager GetInstance()
        {
            return this;
        }
    }
}
