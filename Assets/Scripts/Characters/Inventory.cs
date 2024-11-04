using System;
using Characters;
using Collectibles;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using ModestTree;
using UnityEngine;
using Validation;

public class Inventory : NetworkBehaviour
{
    [SerializeField, SyncVar(hook = nameof(OnInventoryChanged))] private List<HumanDTO> humanDTOs;

    private void OnInventoryChanged(List<HumanDTO> oldValue, List<HumanDTO> newValue)
    {
        Debug.Log("Changed");
    }

    private Action<bool> _onPickedUp;

    private Dictionary<string, List<Vector3>> _staticPickUpCells;
    private CharacterMovement _movement;

    private void Awake()
    {
        _movement = GetComponent<CharacterMovement>();
        _staticPickUpCells = new Dictionary<string, List<Vector3>>();
        humanDTOs = new List<HumanDTO>();
    }

    public void PickUp(Action<bool> onPickedUp)
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

    public bool PickUp(Vector3 cell)
    {
        bool result = false;

        foreach (GameObject entity in CharacterMovement.GetEntities(cell))
        {
            if (entity.TryGetComponent(out ICollectible collectible))
            {
                if (collectible.GetType() == typeof(Human))
                {
                    result = true;
                }
            }
        }

        return result;
    }

    public void AddStaticPickUpCells(List<Vector3> cells, string groupName)
    {
        if (_staticPickUpCells.TryGetValue(groupName, out List<Vector3> groupCells))
        {
            groupCells.AddRange(cells);
            groupCells = groupCells.Distinct().ToList();

            _staticPickUpCells[groupName] = groupCells;
        }
        else
        {
            _staticPickUpCells.Add(groupName, cells);
        }
    }

    private void OnCellChosen(Vector3 cell)
    {
        _onPickedUp(PickUp(cell));
    }

    public bool TryPopItem(out HumanDTO human)
    {
        human = default;
        if (humanDTOs.Count > 0)
        {
            //human = _inventory[0] as Human;
            human = humanDTOs[0];
            humanDTOs.RemoveAt(0);
            //CmdRemoveItem();
            return true;
        }

        return false;
    }

    public void Add(HumanDTO humanDTO)
    {
        humanDTOs.Add(humanDTO);
    }

    public void RemoveStaticPickUpCells(string groupName)
    {
        _staticPickUpCells.Remove(groupName);
    }

    public bool IsStaticPickUpCellsEmpty()
    {
        return _staticPickUpCells.IsEmpty();
    }

    public List<Vector3> GetPickUpCells(int range, Type collectibleType, bool includeStaticCell = true,
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

        if (includeStaticCell)
        {
            foreach (var group in _staticPickUpCells)
            {
                foreach (var cell in group.Value)
                {
                    if (!result.Contains(cell))
                    {
                        if (IsCellPickable(cell, collectibleType))
                        {
                            result.Add(cell);
                        }
                    }
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
