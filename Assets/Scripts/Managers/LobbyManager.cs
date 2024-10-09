using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class LobbyManager : Manager
    {
        [SerializeField] private TMP_InputField joinLobby, createLobby;

        [SerializeField] private UnityEvent onConnectedToLobby,onDisconnectedFromLobby;

        private void OnEnable()
        {
            CustomNetworkManager.OnClientConnected += OnClientConnected;
            CustomNetworkManager.OnClientDisconnected += OnClientDisconnected;
        }

        private void OnDisable()
        {
            CustomNetworkManager.OnClientConnected -= OnClientConnected;
            CustomNetworkManager.OnClientDisconnected -= OnClientDisconnected;
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
            }
            else
            {
                NetworkManager.singleton.StopClient();

                SceneManager.LoadScene(0);
            }
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
