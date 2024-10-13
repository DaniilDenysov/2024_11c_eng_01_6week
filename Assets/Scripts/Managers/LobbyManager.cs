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

namespace Managers
{
    public class LobbyManager : Manager
    {
        public static LobbyManager Instance;
        [SerializeField] private GameObject startGameButton;
        [SerializeField] private TMP_InputField joinLobby, createLobby;
        [SerializeField] private Transform participantsContainer;
        [SerializeField] private UnityEvent onConnectedToLobby,onDisconnectedFromLobby;
        [SerializeField] private List<LobbyCharacterSelector> selectors = new List<LobbyCharacterSelector>();

        public override void Awake()
        {
            base.Awake();
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        private void OnEnable()
        {
            CustomNetworkManager.OnClientConnected += OnClientConnected;
            CustomNetworkManager.OnClientDisconnected += OnClientDisconnected;
            LobbyParticipantHandler.OnPartyOwnerChanged += OnPartyOwnerChanged; 
        }

        public void DeselectAllCharacters ()
        {
            foreach (var selector in selectors)
            {
                selector.SetBlock(false);
            }
        }

        private void OnPartyOwnerChanged(bool obj)
        {
            startGameButton.SetActive(obj);
        }

        public void Ready ()
        {
            NetworkClient.connection.identity.GetComponent<LobbyParticipantHandler>().CmdReady();
        }

        private void OnDisable()
        {
            CustomNetworkManager.OnClientConnected -= OnClientConnected;
            CustomNetworkManager.OnClientDisconnected -= OnClientDisconnected;
            LobbyParticipantHandler.OnPartyOwnerChanged -= OnPartyOwnerChanged;
        }

        public void JoinLobby()
        {
            NetworkManager.singleton.networkAddress = joinLobby.text;
            NetworkManager.singleton.StartClient();
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
            NetworkClient.connection.identity.GetComponent<LobbyParticipantHandler>().CmdStartGame();
        }

        private void OnClientConnected()
        {
            onConnectedToLobby?.Invoke();
        }

        private void OnClientDisconnected()
        {
            DeselectAllCharacters();
            onDisconnectedFromLobby?.Invoke();
        }

        public void CreateLobby()
        {
            NetworkManager.singleton.networkAddress = createLobby.text;
            NetworkManager.singleton.StartHost();
        }
    }
}
