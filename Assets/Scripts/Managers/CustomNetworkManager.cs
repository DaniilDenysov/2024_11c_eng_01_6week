using DTOs;
using System.Linq;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Lobby;

namespace Managers
{
    public class CustomNetworkManager : NetworkManager
    {
        [SerializeField] private NetworkIdentity lobbyParticipant, player;
        [SerializeField] private Transform lobbyDisplay;
        [SerializeField] private List<Player> connectedClients = new List<Player>();
        [SerializeField] private bool isGameInProgress = false;
        public static Action OnClientConnected, OnClientDisconnected;
        public static Action OnValidateStates;


        #region Server

        public override void OnStartServer()
        {
            base.OnStartServer();
            NetworkServer.RegisterHandler<LobbyConnection>(OnCreateCharacter);
        }



        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            base.OnServerConnect(conn);
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);
            if (conn.identity != null)
            {
                connectedClients.RemoveAll((p)=>p.ConnectionId == conn.connectionId);
            }
        }

        
        public void SetSelectionForConnection(NetworkConnection networkConnection, string character)
        {
            var client = connectedClients.Where((c) => c.ConnectionId == networkConnection.connectionId).FirstOrDefault();
            var clientObject = FindObjectsOfType<LobbyParticipantHandler>().Where((p) => p.Player.ConnectionId == client.ConnectionId).FirstOrDefault();
            if (clientObject == null) return;
            if (!string.IsNullOrEmpty(client.CharacterGUID))
            {
                //unblock character
            }
            clientObject.SetCharacterGUID(character); 
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);
          //  var player = conn.identity.GetComponent<LobbyParticipantHandler>();
          //  connectedClients.Add(player);
          //  AddParticipantToContainer(conn);
          //  player.SetPlayerName("Player" + connectedClients.Count);
           // player.SetPartyOwner(connectedClients.Count == 1);

        }


        public override void OnStopServer()
        {
            connectedClients.Clear();
            OnClientDisconnected?.Invoke();
        }

        public override void OnStopHost()
        {
            MessageLogManager.Instance.DisplayMessage("Host error","Host stopped");
            OnClientDisconnected?.Invoke();
        }

        #endregion

        private void OnCreateCharacter(NetworkConnectionToClient conn, LobbyConnection message)
        {
            LobbyParticipantHandler participant = Instantiate(lobbyParticipant).GetComponent<LobbyParticipantHandler>();
            var player = new Player();
            connectedClients.Add(player);
            player.ConnectionId = conn.connectionId;
            player.Nickname = "Player" + connectedClients.Count;
            player.IsPartyOwner = connectedClients.Count == 1;
            NetworkServer.AddPlayerForConnection(conn, participant.gameObject);
            conn.identity.GetComponent<LobbyParticipantHandler>().Player = player;
            var clients = FindObjectsOfType<LobbyParticipantHandler>().OrderBy((c) => c.GetPlayerName());
            foreach (var client in clients)
            {
                client.AddToContainer();
            }
        }

        #region Client

     
        public void AddParticipantToContainer (LobbyParticipantHandler conn)
        {
           conn.transform.SetParent(lobbyDisplay.transform);
        }

        public void StartGame ()
        {

            if (connectedClients.Count >= 1 && isGameInProgress == false)
            {
                foreach (var player in connectedClients)
                {
                    if (!player.IsReady)
                    {
                        return;
                    }
                }
                isGameInProgress = true;
                //change to more appropriate handling
                ServerChangeScene("SandBox");
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
            connectedClients.Clear();
            MessageLogManager.Instance.DisplayMessage("Host error", "Host stopped");
            OnClientDisconnected?.Invoke();
        }
        #endregion
    }

    public struct LobbyConnection : NetworkMessage
    {
      
    }
}
