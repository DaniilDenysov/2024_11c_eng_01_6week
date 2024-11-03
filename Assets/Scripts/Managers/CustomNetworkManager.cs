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
using UI.PlayerLog;

namespace Managers
{
    public class CustomNetworkManager : NetworkManager
    {
        [SerializeField] private List<Player> players;
        [SerializeField, Range(1, 4)] private int minimalLobbySize = 1;
        [SerializeField] private GameObject playerLabelPrefab;
        [SerializeField] private PlayerScoreLabel playerScoreLabel;
        [SerializeField] private string mainScene;
        [SerializeField] private bool isGameInProgress = false;
        public static Action OnClientConnected, OnClientDisconnected;

        public override void Awake()
        {
            base.Awake();
            Inventory.OnHumanPickedUp += OnHumanPickedUp;
        }

        [Server]
        private void OnHumanPickedUp()
        {
            foreach (var player in NetworkPlayerContainer.Instance.GetItems())
            {
                if (player.gameObject.TryGetComponent(out Inventory inv))
                {
                    //change to == 12 later
                    if (inv.GetHumans().Count >= 1)
                    {
                        GameplayManager.Instance.OnGameFinished();
                        foreach (var score in NetworkPlayerContainer.Instance.GetItems())
                        {
                            PlayerScoreLabel playerScore = Instantiate(playerScoreLabel, PlayerScoreContainer.Instance.transform);
                            PlayerScoreContainer.Instance.Add(playerScore);
                            var playerData = score.GetPlayerData();
                            playerScore.Construct(player.GetCharacterData(),playerData.Nickname, $"{NetworkPlayerContainer.Instance.CalculateToatlScoreForPlayer(score)} cal");
                        }
                        break;
                    }
                }
            }
        }


        #region Server



        public override void OnStartServer()
        {
            base.OnStartServer();
            NetworkServer.maxConnections = 4;
            NetworkServer.RegisterHandler<LobbyConnection>(OnCreateCharacter);
        }

        private void OnRecievedMessage(NetworkConnectionToClient arg1, ServerMessage arg2)
        {
            MessageLogManager.Instance.DisplayMessage(arg2.Title, arg2.Description);
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            base.OnServerConnect(conn);
            //assign to available
            Debug.Log("Connections:"+ NetworkServer.connections.Count);
            if (isGameInProgress && NetworkServer.connections.Count <= NetworkServer.maxConnections)
            {
                var networkPlayer = GetPlayerForConnection(conn.connectionId);
                if (networkPlayer == null)
                {
                    networkPlayer = NetworkPlayerContainer.Instance.GetItems().FirstOrDefault((p) => !NetworkServer.connections.Keys.Contains(p.GetPlayerData().ConnectionId));
                }
                //refactor with try functions
                if (networkPlayer != null)
                {
                    var playerData = networkPlayer.GetPlayerData();
                    playerData.ConnectionId = conn.connectionId;
                    playerData.ConnectionId = conn.connectionId;
                    players[players.IndexOf(players.FirstOrDefault((p)=>p.CharacterGUID== playerData.CharacterGUID))] = playerData;
                    NetworkServer.AddPlayerForConnection(conn, networkPlayer.gameObject);
                    Debug.Log($"assigned {networkPlayer.GetCharacterGUID()} for connection {conn.connectionId}");
                    return;
                }
                conn.Disconnect();
            }
  
        }

        public NetworkPlayer GetPlayerForConnection(int connectionId)
        {
            var playerData = players.FirstOrDefault((p) => p.ConnectionId == connectionId);
            if (playerData != null)
            {
                var networkPlayer = NetworkPlayerContainer.Instance.GetItems().FirstOrDefault((p) => p.GetPlayerData().ConnectionId == connectionId);
                return networkPlayer;
            }
            return null;
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            base.OnServerSceneChanged(sceneName);
            if (isGameInProgress)
            {
                AssignOwnersForConnections();
            }
        }

        public override void OnServerChangeScene(string newSceneName)
        {
            base.OnServerChangeScene(newSceneName);
        }

        public override void OnClientSceneChanged()
        {
            base.OnClientSceneChanged();
        }

        private void AssignOwnersForConnections ()
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
                    NetworkServer.AddPlayerForConnection(NetworkServer.connections[playerData.ConnectionId], networkPlayer.gameObject);
                }
            }
            Debug.Log("Assigned ownership!");
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);
            if (!isGameInProgress)
            {
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
            else
            {
                var networkPlayer = GetPlayerForConnection(conn.connectionId);
                if (networkPlayer != null)
                {
                    NetworkServer.RemovePlayerForConnection(conn, networkPlayer.gameObject);
                    NetworkServer.connections.Remove(conn.connectionId);
                }
            }
           /* else
            {
                StopServer();
            }*/
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
            PlayerLabel [] connections = PlayerLabelsContainer.Instance.GetItems().ToArray();
            if (participant.gameObject.TryGetComponent(out PlayerLabel label))
            {
                var player = new Player();
                player.ConnectionId = conn.connectionId;
                string name = PlayerPrefs.GetString("Nickname");
                if (string.IsNullOrEmpty(name)) player.Nickname = "Player" + connections.Length;
                else player.Nickname = name;
                player.IsPartyOwner = connections.Length == 0;
                label.Player = player;
            }
            NetworkServer.AddPlayerForConnection(conn, participant);
        }

        #region Client

        public void StartGame ()
        {
            var connections = FindObjectsOfType<PlayerLabel>();
            if (connections.Length >= minimalLobbySize && isGameInProgress == false)
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
                isGameInProgress = true;
                players = GetPlayersData();
                //change to more appropriate handling
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

    public struct LobbyConnection : NetworkMessage
    {
      
    }

    public struct ServerMessage : NetworkMessage
    {
        public string Title, Description;
    }
}
