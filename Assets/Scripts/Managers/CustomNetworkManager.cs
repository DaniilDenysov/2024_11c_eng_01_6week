using DTOs;
using System.Linq;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Lobby;
using Zenject;
using UI;
using General;

namespace Managers
{
    public class CustomNetworkManager : NetworkManager
    {
        [SerializeField] private List<Player> players;
        [SerializeField] private GameObject playerLabelPrefab;
        [SerializeField] private bool isGameInProgress = false;
        public static Action OnClientConnected, OnClientDisconnected;

        #region Server

        public override void OnStartServer()
        {
            base.OnStartServer();
            NetworkServer.RegisterHandler<LobbyConnection>(OnCreateCharacter);
        }

        private void OnRecievedMessage(NetworkConnectionToClient arg1, ServerMessage arg2)
        {
            MessageLogManager.Instance.DisplayMessage(arg2.Title, arg2.Description);
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            base.OnServerConnect(conn);
        }

        public override void OnServerChangeScene(string newSceneName)
        {
            base.OnServerChangeScene(newSceneName);
            Invoke(nameof(DelayedAction),5f);
        }

        public void DelayedAction ()
        {
            StopServer();
        }

        public override void OnClientSceneChanged()
        {
            base.OnClientSceneChanged();
            if (isGameInProgress)
            {
                Dictionary<string, Player> characterMappings = new Dictionary<string, Player>();
                foreach (var player in players)
                {
                    characterMappings.TryAdd(player.CharacterGUID, player);
                }
                foreach (var networkPlayer in NetworkPlayerContainer.Instance.GetItems())
                {
                    if (characterMappings.TryGetValue(networkPlayer.GetCharacterGUID(), out Player playerData))
                    {
                        networkPlayer.SetPlayer(playerData);
                        NetworkServer.AddPlayerForConnection(NetworkServer.connections[playerData.ConnectionId],networkPlayer.gameObject);
                    }
                }
            }
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);
            var participants = FindObjectsOfType<PlayerLabel>();
            foreach (var participant in participants)
            {
                participant.OnValidateSelection();
            }
            foreach (var participant in participants)
            {
                participant.ValidateSelection();
            }
        }

        public ServerMessage ErrorMessage (string title,string description)
        {
            var message = new ServerMessage();
            message.Description = description;
            message.Title = title;
            return message;
        }

        public override void OnStopServer()
        {
            OnClientDisconnected?.Invoke();
        }

        public override void OnStopHost()
        {
            OnClientDisconnected?.Invoke();
        }

        #endregion

        private List<Player> GetPlayersData ()
        {
            return PlayerLabelsContainer.Instance.GetItems().Select((i)=>i.Player).ToList();
        }

        private void OnCreateCharacter(NetworkConnectionToClient conn,LobbyConnection lobbyConnection)
        {
            GameObject participant = Instantiate(playerLabelPrefab);
            if (participant != null)
            {
                Debug.Log("PlayerLabel prefab instantiated successfully.");  // Debug log
            }
            PlayerLabel [] connections = PlayerLabelsContainer.Instance.GetItems().ToArray();
            if (participant.gameObject.TryGetComponent(out PlayerLabel label))
            {
                var player = new Player();
                player.ConnectionId = conn.connectionId;
                player.Nickname = "Player" + connections.Length;
                player.IsPartyOwner = connections.Length == 0;
                label.Player = player;
            }
            else
            {
                Debug.LogError("Unable to get label!");
            }
            NetworkServer.AddPlayerForConnection(conn, participant);
        }

        #region Client

        public void StartGame ()
        {
            var connections = FindObjectsOfType<PlayerLabel>();
            if (connections.Length >= 1 && isGameInProgress == false)
            {
                foreach (var player in connections.Select((h) => h.Player).ToList())
                {
                    if (!player.IsReady) return;
                }
                isGameInProgress = true;
                players = GetPlayersData();
                //change to more appropriate handling
                ServerChangeScene("Main");
            }
        }

        public override void OnClientConnect()
        {
            base.OnClientConnect();
            NetworkClient.Send(new LobbyConnection());
            OnClientConnected?.Invoke();
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            OnClientDisconnected?.Invoke();
        }

        public override void OnStopClient()
        {
            OnClientDisconnected?.Invoke();
        }
        #endregion
    }

    public struct LobbyConnection : NetworkMessage
    {
      
    }

    public struct ServerMessage : NetworkMessage
    {
        public string Title, Description;
    }
}
