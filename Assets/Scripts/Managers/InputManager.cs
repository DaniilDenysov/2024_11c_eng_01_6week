using DesignPatterns.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions.Vector;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class InputManager : Singleton<InputManager>
{
    [SerializeField] private List<Item> cellCallbacks = new List<Item>();
    [SerializeField] private List<Vector3> positions = new List<Vector3>();
    private Action<Vector3Int> onTileSelected;
    private Tilemap tilemap;

    public override InputManager GetInstance()
    {
        return this;
    }

    private InputActions inputActions;

    public override void Awake()
    {
        base.Awake();
        tilemap = GetComponent<Tilemap>();
        inputActions = new InputActions();
        inputActions.Enable();
        inputActions.Player.LeftMouseButton.performed += OnPressed;
    }

    private void OnPressed(InputAction.CallbackContext obj)
    {
        var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPoint.z = 0;
        var tilePos = WorldToTile(worldPoint);

        Debug.Log($"Pressed: {tilePos}, Cell count: {cellCallbacks.Count}");
        // Find the item in the list with the corresponding position
        var item = cellCallbacks.Find(i => i.position == tilePos);

        if (item.func != null)
        {
            item.func.Invoke(TileToWorld(tilePos));
            cellCallbacks.Remove(item);
            UpdatePositionsList();
        }
    }

    public void AddCellCallbackOverride(Vector3 cell, Action<Vector3> func)
    {
        Debug.Log($"Cell: {cell}");
        // Remove any existing callback for the same position, then add the new one
        cellCallbacks.RemoveAll(i => i.position == cell);
        cellCallbacks.Add(new Item { position = WorldToTile(cell), func = func });
        UpdatePositionsList();
        Debug.Log($"Cell count: {cellCallbacks.Count}");
    }

    public void AddCellCallbacksOverride(HashSet<Vector3> cells, Action<Vector3> func)
    {
        foreach (var cell in cells)
        {
            AddCellCallbackOverride(cell, func);
        }
        UpdatePositionsList();
    }

    public void AddCellCallback(Vector3Int cell, Action<Vector3> func)
    {
        // Check if there's an existing item for the cell and update the function
        var existingItem = cellCallbacks.Find(i => i.position == cell);

        if (existingItem.func != null)
        {
            existingItem.func += func;
        }
        else
        {
            cellCallbacks.Add(new Item { position = cell, func = func });
        }
        UpdatePositionsList();
    }

    public void AddCellCallbacks(HashSet<Vector3> cells, Action<Vector3> func)
    {
        foreach (var cell in cells)
        {
            AddCellCallback(WorldToTile(cell), func);
        }
    }

    public Vector3Int WorldToTile(Vector3 pos)
    {
        return tilemap.WorldToCell(pos);
    }

    public Vector3 TileToWorld(Vector3Int tile)
    {
        return tilemap.CellToWorld(tile) + tilemap.cellSize / 2f;
    }

    public void ClearCallbacks()
    {
        cellCallbacks.Clear();
        UpdatePositionsList();
    }

    private void UpdatePositionsList()
    {
        positions.Clear();
        foreach (var item in cellCallbacks)
        {
            positions.Add(item.position);
        }
    }

    // Draw gizmos for each position in the list.
    private void OnDrawGizmos()
    {
        if (tilemap == null || positions == null) return;

        Gizmos.color = Color.cyan; // Set the color for the gizmos.

        foreach (var position in positions)
        {
            // Convert the tile position to a world position and draw a sphere at each position.
            Vector3 worldPos = TileToWorld(position.VectorToIntVector());
            Gizmos.DrawSphere(worldPos, 0.3f);
        }
    }

    [Serializable]
    struct Item
    {
        public Vector3 position;
        public Action<Vector3> func;
    }
}
