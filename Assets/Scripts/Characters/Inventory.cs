using Characters;
using Collectibles;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour, ICollector
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private List<ICollectible> inventory = new List<ICollectible>();

    public bool PickUp()
    {
        var result = Physics2D.OverlapCircle(transform.position, 1f, layerMask);
        if (result != null && result.TryGetComponent(out ICollectible collectible))
        {
            var collected = collectible.Collect();
            if (collected != default && collectible.GetType() == typeof(Human))
            {
                inventory.Add(collected as Human);
                return true;
            }
        }
        return false;
    }

    public bool TryPopItem (out Human human)
    {
        human = default;
        if (inventory.Count > 0)
        {
            human = inventory[0] as Human;
            inventory.RemoveAt(0);
            return true;
        }
        return false;
    }

    public void Add(Human human)
    {
        inventory.Add(human);
    }
}
