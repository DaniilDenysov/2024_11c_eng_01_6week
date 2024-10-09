using Managers;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyParticipantHandler : MonoBehaviour
{
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

    private void OnClientDisconnected()
    {

    }

    private void OnClientConnected()
    {

    }

}
