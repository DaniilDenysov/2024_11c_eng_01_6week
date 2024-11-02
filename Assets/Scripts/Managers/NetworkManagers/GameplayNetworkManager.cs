using DTOs;
using General;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;

namespace Managers.Network
{
    public class GameplayNetworkManager : NetworkManager
    {
        #region Server

        public override void OnStartServer()
        {
            base.OnStartServer();
            NetworkServer.maxConnections = 4;
        }

        private void OnRecievedMessage(NetworkConnectionToClient arg1, ServerMessage arg2)
        {
            MessageLogManager.Instance.DisplayMessage(arg2.Title, arg2.Description);
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);
            Debug.Log("Added");
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            base.OnServerConnect(conn);
            
            //assign to available
            Debug.Log("Connections:" + NetworkServer.connections.Count);
            if (NetworkServer.connections.Count <= NetworkServer.maxConnections)
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
                    GetPlayers()[GetPlayers().IndexOf(GetPlayers().FirstOrDefault((p) => p.CharacterGUID == playerData.CharacterGUID))] = playerData;
                    NetworkServer.AddPlayerForConnection(conn, networkPlayer.gameObject);
                    Debug.Log($"assigned {networkPlayer.GetCharacterGUID()} for connection {conn.connectionId}");
                    return;
                }
                conn.Disconnect();
            }

        }

        public List<Player> GetPlayers() => ClientDataManager.Instance.GetPlayers().ToList();

        public NetworkPlayer GetPlayerForConnection(int connectionId)
        {
            var playerData = GetPlayers().FirstOrDefault((p) => p.ConnectionId == connectionId);
            if (playerData != null)
            {
                var networkPlayer = NetworkPlayerContainer.Instance.GetItems().FirstOrDefault((p) => p.GetPlayerData().ConnectionId == connectionId);
                return networkPlayer;
            }
            return null;
        }

        private void AssignOwnersForConnections()
        {
            Dictionary<string, Player> characterMappings = new Dictionary<string, Player>();
            foreach (var player in GetPlayers())
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

                var networkPlayer = GetPlayerForConnection(conn.connectionId);
                if (networkPlayer != null)
                {
                    NetworkServer.RemovePlayerForConnection(conn, networkPlayer.gameObject);
                    NetworkServer.connections.Remove(conn.connectionId);
                }
        }

        public ServerMessage ErrorMessage(string title, string description)
        {
            var message = new ServerMessage();
            message.Description = description;
            message.Title = title;
            return message;
        }
        #endregion

        private List<Player> GetPlayersData()
        {
            return PlayerLabelsContainer.Instance.GetItems().Select((i) => i.Player).ToList();
        }


        #region Client

        

        public override void OnClientConnect()
        {
            base.OnClientConnect();
            NetworkClient.Send(new LobbyConnection());
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
