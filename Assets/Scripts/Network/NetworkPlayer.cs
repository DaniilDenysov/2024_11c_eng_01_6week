using DTOs;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
   
    [SerializeField,SyncVar] private Player player;
    [SerializeField] private CharacterData characterData;


    public string GetCharacterGUID() => characterData.CharacterGUID;

    [Server]
    public void SetPlayer(Player player)
    {
        this.player = player;
    }
}
