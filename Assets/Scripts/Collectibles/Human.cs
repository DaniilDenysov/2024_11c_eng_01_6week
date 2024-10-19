using Collectibles;
using CustomTools;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : NetworkBehaviour, ICollectible
{
    [SerializeField,SyncVar] private string ownedBy;
    [SerializeField, Range(2,6)] private int currentPoints; 

    [Server]
    public void SetOwner (string owner)
    {
        ownedBy = owner;
    }

    public object Collect()
    {
        gameObject.SetActive(false);
        return this;
    }
}
