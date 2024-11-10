using System;
using Characters;
using Collectibles;
using System.Collections.Generic;
using Mirror;
using UI.Containers;
using UnityEngine;
using Validation;

public class Inventory : NetworkBehaviour
{
    [SerializeField] private SyncList<HumanDTO> humanDTOs = new SyncList<HumanDTO>();
    private CharacterMovement _movement;
    public static Action OnHumanPickedUp;

    public virtual void Awake()
    {
        _movement = GetComponent<CharacterMovement>();
        humanDTOs.OnAdd += OnInventoryChanged;
    }

    private void OnInventoryChanged(int itemIndex)
    {
        Debug.Log("Picked");
        OnHumanPickedUp?.Invoke();
    }

    public virtual void PickUp(Action<bool> onPickedUp)
    {
        onPickedUp.Invoke(PickUp(transform.position));
    }

    public bool PickUp(Vector3 cell)
    {
        bool result = false;
        bool isGameEnded = false;

        foreach (GameObject entity in CharacterMovement.GetEntities(cell))
        {
            if (entity.TryGetComponent(out ICollectible collectible))
            {
                if (collectible.GetType() == typeof(Human))
                {
                    HumanDTO humanDTO = (collectible as Human).GetData();

                    AddHuman(humanDTO);
                    collectible.Collect();
                    if (!InventoryContainer.Instance.TryAdd(humanDTO))
                    {
                        isGameEnded = true;
                    }
                    result = true;
                }
            }
        }

        if (isGameEnded)
        {
            Debug.Log("YEEAHH, Game ended!!!");
        }

        return result;
    }

    public bool TryPopItem(out HumanDTO human)
    {
        human = default;
        if (humanDTOs.Count > 0)
        {
            human = humanDTOs[0];
            if (!InventoryContainer.Instance.TryRemove()) return false;
            RemoveHuman();
            return true;
        }

        return false;
    }

    public void AddHuman(HumanDTO humanDTO)
    {
        if (!isOwned)
        {
            Debug.Log("CANT MODIFY");
            return;
        }
        humanDTOs.Add(humanDTO);
        Debug.Log("Added");
    }

    public void RemoveHuman()
    {
        if (!isOwned)
        {
            Debug.Log("CANT MODIFY");
            return;
        }
        humanDTOs.RemoveAt(0);
    }

    public IReadOnlyCollection<HumanDTO> GetHumans() => humanDTOs;

    public virtual List<Vector3> GetPickUpCells(int range, Type collectibleType, bool includeStaticCell = true,
        bool includeCurrentCell = true)
    {
        PathValidator pathValidator = PathValidator.Instance;
        Vector3 characterPosition = transform.position;
        List<Vector3> directions =
            includeCurrentCell ? CharacterMovement.GetAttackDirections() : CharacterMovement.GetAllDirections();

        List<Vector3> result = new List<Vector3>();

        foreach (Vector3 direction in directions)
        {
            for (int distance = 0; distance < range; distance++)
            {
                Vector3 currentCell = characterPosition + direction * (distance + 1);

                if (!pathValidator.CanMoveTo(characterPosition, currentCell))
                {
                    break;
                }

                if (IsCellPickable(currentCell, collectibleType))
                {
                    result.Add(currentCell);
                }
            }
        }

        return result;
    }

    public bool IsCellPickable(Vector3 cell, Type collectibleType)
    {
        foreach (GameObject entity in CharacterMovement.GetEntities(cell))
        {
            if (IsPickable(entity, collectibleType))
            {
                return true;
            }
        }

        return false;
    }

    public bool IsPickable(GameObject entity, Type collectibleType)
    {
        return entity.TryGetComponent(out ICollectible collectible)
               && collectible.GetType() == collectibleType;
    }
}