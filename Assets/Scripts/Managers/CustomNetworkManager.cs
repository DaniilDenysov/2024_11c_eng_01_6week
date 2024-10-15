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
           // NetworkServer.RegisterHandler<ServerMessage>(OnRecievedMessage);
        }

        private void OnRecievedMessage(NetworkConnectionToClient arg1, ServerMessage arg2)
        {
            Debug.Log("Revieved message");
            MessageLogManager.Instance.DisplayMessage(arg2.Title, arg2.Description);
        }

        public void UpdateSelection (List<int> connectionIds)
        {
           foreach(var handler in FindObjectsOfType<LobbyParticipantHandler>())
           {
                if (connectionIds.Contains(handler.Player.ConnectionId))
                {
                    Debug.Log("a");
                    LobbyCharacterSelector.OnSelected?.Invoke(handler.Player.CharacterGUID);
                }
                else
                {
                    Debug.Log("b");
                    LobbyCharacterSelector.OnDeselected?.Invoke(handler.Player.CharacterGUID);
                }
           }
            Debug.Log("Updated selections");
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            base.OnServerConnect(conn);
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);
            // NetworkClient.connection.identity.GetComponent<LobbyParticipantHandler>().DeselectOnClient();
           // NetworkServer.SendToAll(ErrorMessage("Host error", "Host left the lobby"));


            OnValidateStates?.Invoke();
            // OnValidateStates?.Invoke();

        }

        public void OnLeave ()
        {
            
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
            connectedClients.Clear();
           // NetworkServer.SendToAll(ErrorMessage("Host error", "Host left the lobby"));
            OnClientDisconnected?.Invoke();
        }

        public override void OnStopHost()
        {
         //   NetworkServer.SendToAll(ErrorMessage("Host error", "Host left the lobby"));
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
            Debug.Log("Start game");
            if (connectedClients.Count >= 1 && isGameInProgress == false)
            {
                foreach (var player in FindObjectsOfType<LobbyParticipantHandler>().Select((h) => h.Player).ToList())
                {
                    if (!player.IsReady)
                    {
                        Debug.Log("Not rdy!");
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
            connectedClients = FindObjectsOfType<LobbyParticipantHandler>().Select((h) => h.Player).ToList();
            NetworkClient.Send(new LobbyConnection());
            OnClientConnected?.Invoke();
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
        //    LobbyManager.Instance.DeselectAllCharacters();
         //   OnValidateStates?.Invoke();
            OnClientDisconnected?.Invoke();
        }

        public override void OnStopClient()
        {
            connectedClients.Clear();
            LobbyManager.Instance.DeselectAllCharacters();
            //MessageLogManager.Instance.DisplayMessage("Host error", "Host stopped");
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
