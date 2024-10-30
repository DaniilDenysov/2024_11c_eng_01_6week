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
    [SerializeField] private List<HumanDTO> humanDTOs;
    private CharacterMovement _movement;

    private void Awake()
    {
        _movement = GetComponent<CharacterMovement>();
        humanDTOs = new List<HumanDTO>();
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
                    
                    CmdAddHuman(humanDTO);
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
            CmdRemoveHuman();
            return true;
        }

        return false;
    }
    
    [Command(requiresAuthority = false)]
    public void CmdRemoveHuman()
    {
        RpcAddHuman();
    }

    [ClientRpc]
    public void RpcAddHuman()
    {
        humanDTOs.RemoveAt(0);
    }

    [Command(requiresAuthority = false)]
    public void CmdAddHuman(HumanDTO humanDTO)
    {
        RpcAddHuman(humanDTO);
    }

    [ClientRpc]
    public void RpcAddHuman(HumanDTO humanDTO)
    {
        humanDTOs.Add(humanDTO);
    }

    public virtual List<Vector3> GetPickUpCells(int range, Type collectibleType, bool includeStaticCell = true,
        bool includeCurrentCell = true)
    {
        PathValidator pathValidator = _movement.GetPathValidator();
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
