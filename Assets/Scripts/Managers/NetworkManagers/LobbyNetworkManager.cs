using DTOs;
using General;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;

namespace Managers.Network
{
    public class LobbyNetworkManager : NetworkManager
    {
        [SerializeField, Range(1, 4)] private int minimalLobbySize = 1;
        [SerializeField] private GameObject playerLabelPrefab;
        [SerializeField] private string mainScene;
        public static Action OnClientConnected, OnClientDisconnected;

        #region Server

        public override void OnStartServer()
        {
            base.OnStartServer();
            NetworkServer.maxConnections = 4;
            NetworkServer.RegisterHandler<LobbyConnection>(OnCreateCharacter);
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

        public ServerMessage ErrorMessage(string title, string description)
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

        private List<Player> GetPlayersData()
        {
            return PlayerLabelsContainer.Instance.GetItems().Select((i) => i.Player).ToList();
        }

        private void OnCreateCharacter(NetworkConnectionToClient conn, LobbyConnection lobbyConnection)
        {
            GameObject participant = Instantiate(playerLabelPrefab);
            PlayerLabel[] connections = PlayerLabelsContainer.Instance.GetItems().ToArray();
            if (participant.gameObject.TryGetComponent(out PlayerLabel label))
            {
                var player = new Player();
                player.ConnectionId = conn.connectionId;
                string name = PlayerPrefs.GetString("Nickname");
                if (string.IsNullOrEmpty(name)) player.Nickname = "Player" + connections.Length;
                else player.Nickname = name;
                player.IsPartyOwner = connections.Length == 0;
                label.Player = player;
                ClientDataManager.Instance.AddPlayer(player);
            }
            NetworkServer.AddPlayerForConnection(conn, participant);
        }

        #region Client

        public void StartGame()
        {
            var connections = FindObjectsOfType<PlayerLabel>();
            if (connections.Length >= minimalLobbySize)
            {
                foreach (var player in connections.Select((h) => h.Player).ToList())
                {
                    if (!player.IsReady)
                    {
                        LocalPlayerLogContainer.Instance.AddLogMessage("Not all players are ready!");
                        return;
                    }
                    if (string.IsNullOrEmpty(player.CharacterGUID))
                    {
                        LocalPlayerLogContainer.Instance.AddLogMessage("Not all players selected their character!");
                        return;
                    }
                }
                ServerChangeScene(mainScene);
            }
            else
            {
                LocalPlayerLogContainer.Instance.AddLogMessage("Not enough players!");
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

}
