using System;
using Characters;
using Collectibles;
using System.Collections.Generic;
using Ganeral;
using UnityEngine;

public class Inventory : MonoBehaviour, ICollector
{
    [SerializeField] private LayerMask layerMask;
    private List<ICollectible> _inventory;

    private void Awake()
    {
        _inventory = new List<ICollectible>();
    }

    public bool PickUp()
    {
        return PickUp(transform.position);
    }
    
    public bool PickUp(Vector3 cell)
    {
        bool result = false;
        
        foreach (GameObject entity in CoordinateManager.GetEntities(cell))
        {
            if (entity.TryGetComponent(out ICollectible collectible))
            {
                if (collectible.GetType() == typeof(Human))
                {
                    var collected = collectible.Collect();
                    _inventory.Add(collected as Human);
                    result = true;
                }
            }
        }
        return result;
    }

    public bool TryPopItem (out Human human)
    {
        human = default;
        if (_inventory.Count > 0)
        {
            human = _inventory[0] as Human;
            _inventory.RemoveAt(0);
            return true;
        }
        return false;
    }

    public void Add(Human human)
    {
        _inventory.Add(human);
    }
}
