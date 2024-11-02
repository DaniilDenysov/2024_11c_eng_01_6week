using DesignPatterns.Singleton;
using DTOs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientDataManager : Singleton<ClientDataManager>
{
    private HashSet<Player> players = new HashSet<Player>();

    public override void Awake()
    {
        base.Awake();
        if (Instance = this)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public override ClientDataManager GetInstance()
    {
        return this;
    }

    public void AddPlayer(Player player)
    {
        if (!players.Add(player))
        {
            Debug.Log("Unable to add player!");
        }
    }

    public void RemovePlayer(Player player)
    {
        if (!players.Remove(player))
        {
            Debug.Log("Unable to remove player!");
        }
    }

    public void AddPlayers (ICollection<Player> players)
    {
        this.players = new HashSet<Player>(players);
    }

    public IReadOnlyCollection<Player> GetPlayers() => players;
}
