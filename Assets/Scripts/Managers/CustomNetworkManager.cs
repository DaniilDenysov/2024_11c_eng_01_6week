using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class CustomNetworkManager : NetworkManager
    {
        [SerializeField] private NetworkIdentity lobbyParticipant, player;
        [SerializeField] private Transform lobbyDisplay;
        [SerializeField] private List<LobbyParticipantHandler> connectedClients = new List<LobbyParticipantHandler>();
        [SerializeField] private bool isGameInProgress = false;
        public static Action OnClientConnected, OnClientDisconnected;



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
            if (conn.identity != null && conn.identity.TryGetComponent(out LobbyParticipantHandler player))
            {
                connectedClients.Remove(player);
            }
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
            connectedClients.Add(participant);
            NetworkServer.AddPlayerForConnection(conn, participant.gameObject);
            foreach (var player in connectedClients)
            {
                player.AddToContainer();
            }
            participant.SetPlayerName("Player" + connectedClients.Count);
            participant.SetPartyOwner(connectedClients.Count == 1);
        }

        #region Client

     
        public void AddParticipantToContainer (LobbyParticipantHandler conn)
        {
           conn.transform.SetParent(lobbyDisplay.transform);
        }

        public void StartGame ()
        {

            if (connectedClients.Count >= 2 && isGameInProgress == false)
            {
                foreach (var player in connectedClients)
                {
                    if (!player.GetReady())
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
