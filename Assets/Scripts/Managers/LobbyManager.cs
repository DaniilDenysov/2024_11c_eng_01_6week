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
