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
using Managers.Network;

namespace Managers
{
    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager Instance;
        [SerializeField] private GameObject startGameButton;
        [SerializeField] private string lobbyName;
        [SerializeField] private UnityEvent onConnectedToLobby,onDisconnectedFromLobby;


        private void Awake()
        {
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
          //  LobbyNetworkManager.OnClientConnected += OnClientConnected;
          //  LobbyNetworkManager.OnClientDisconnected += OnClientDisconnected;
            PlayerLabel.OnPartyOwnerChanged += OnPartyOwnerChanged; 
        }

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

        private void OnDisable()
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
        }

        private void OnClientDisconnected()
        {
            CharacterSelectionLabelContainer.Instance.DeselectAllCharacters();
            onDisconnectedFromLobby?.Invoke();
        }

        public void CreateLobby()
        {
            NetworkManager.singleton.networkAddress = lobbyName;
            NetworkManager.singleton.StartHost();
        }
    }
}
