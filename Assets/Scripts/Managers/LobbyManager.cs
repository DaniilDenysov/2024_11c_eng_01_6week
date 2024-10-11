using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System;

namespace Managers
{
    public class LobbyManager : Manager
    {
        [SerializeField] private GameObject startGameButton;
        [SerializeField] private TMP_InputField joinLobby, createLobby;
        [SerializeField] private Transform participantsContainer;
        [SerializeField] private UnityEvent onConnectedToLobby,onDisconnectedFromLobby;

        private void OnEnable()
        {
            CustomNetworkManager.OnClientConnected += OnClientConnected;
            CustomNetworkManager.OnClientDisconnected += OnClientDisconnected;
            LobbyParticipantHandler.OnPartyOwnerChanged += OnPartyOwnerChanged; 
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

        public void LeaveLobby ()
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
            onDisconnectedFromLobby?.Invoke();
        }

        public void CreateLobby()
        {
            NetworkManager.singleton.networkAddress = createLobby.text;
            NetworkManager.singleton.StartHost();
        }
    }
}
