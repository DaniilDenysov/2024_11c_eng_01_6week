using System;
using Characters;
using Collectibles;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : Collector
{
    [SerializeField] private List<HumanDTO> humanDTOs;
    [SerializeField] private List<ICollectible> _inventory;
    [SerializeField] private int cardDrawCount = 1;
    private Action<bool> _onPickedUp;

    private new void Awake()
    {
        base.Awake();
        _inventory = new List<ICollectible>();
        humanDTOs = new List<HumanDTO>();
    }

    public override void PickUp(Action<bool> onPickedUp)
    {
        if (IsStaticPickUpCellsEmpty())
        {
            onPickedUp.Invoke(PickUp(transform.position));
        }
        else
        {
            List<Vector3> litPositions = GetPickUpCells(0, typeof(Human));
            TileSelector.Instance.SetTilesLit(litPositions, OnCellChosen);
            _onPickedUp = onPickedUp;
        }
    }

    public override bool PickUp(Vector3 cell)
    {
        bool result = false;

        foreach (GameObject entity in CharacterMovement.GetEntities(cell))
        {
            if (entity.TryGetComponent(out ICollectible collectible))
            {
                if (collectible.GetType() == typeof(Human))
                {
                    var collected = collectible.Collect();
                    _inventory.Add(collected as Human);
                    humanDTOs.Add((collected as Human).GetData());
                    result = true;
                }
            }
        }

        return result;
    }

    private void OnCellChosen(Vector3 cell)
    {
        _onPickedUp(PickUp(cell));
    }

    public bool TryPopItem(out Human human)
    {
        human = default;
        if (_inventory.Count > 0)
        {
            human = _inventory[0] as Human;
            _inventory.RemoveAt(0);
            humanDTOs.RemoveAt(0);
            return true;
        }
        return false;
    }

    public void AdjustCardDraw(int adjustment)
    {
        cardDrawCount += adjustment;
        cardDrawCount = Mathf.Max(1, cardDrawCount);
    }

    public void Add(Human human)
    {
        _inventory.Add(human);
        humanDTOs.Add(human.GetData());
    }
}
