using DesignPatterns.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class InputManager : Singleton<InputManager>
{
    private IDictionary<Vector3Int, Action<Vector3>> cellCallbacks = new Dictionary<Vector3Int, Action<Vector3>>();
    [SerializeField] private List<Vector3Int> positions = new List<Vector3Int>();
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
    }

    public void OnMouseUp()
    {
        var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPoint.z = 0;
        var tilePos = WorldToTile(worldPoint);
        Debug.Log("TilePos:"+tilePos);
        Debug.Log("AddedCellsOverride " + cellCallbacks.Keys.Count);
        if (cellCallbacks.TryGetValue(tilePos, out Action<Vector3> action))
        {
            Debug.Log("TilePosToWorld:" + TileToWorld(tilePos));
            action.Invoke(TileToWorld(tilePos));
            cellCallbacks.Remove(tilePos);
            positions = new List<Vector3Int>(cellCallbacks.Keys);
        }
    }

    public void AddCellCallbackOverride(Vector3Int cell,Action<Vector3> func)
    {
        cellCallbacks[cell] = func;
    }

    public void AddCellCallbacksOverride(HashSet<Vector3> cells, Action<Vector3> func)
    {
        foreach (var cell in cells)
        {
            AddCellCallbackOverride(WorldToTile(cell),func);
        }
        Debug.Log("AddedCellsOverride " + cellCallbacks.Keys.Count);
       // positions = new List<Vector3Int>(cellCallbacks.Keys);
    }

    public void AddCellCallback(Vector3Int cell, Action<Vector3> func)
    {
       Action<Vector3> oldFunc;
        if (cellCallbacks.TryGetValue(cell,out oldFunc))
        {
            oldFunc += func;
        }
        else
        {
            cellCallbacks.Add(cell,func);
        }
    }

    public void AddCellCallbacks(HashSet<Vector3> cells, Action<Vector3> func)
    {
        foreach (var cell in cells)
        {
            AddCellCallback(WorldToTile(cell), func);
        }
        Debug.Log("afsgaasrh");
    }

    public Vector3Int WorldToTile (Vector3 pos)
    {
        return tilemap.WorldToCell(pos);
    }

    public Vector3 TileToWorld(Vector3Int tile)
    {
        return tilemap.CellToWorld(tile)+ tilemap.cellSize / 2f;
    }

    public void ClearCallbacks()
    {
        cellCallbacks.Clear();
        Debug.Log("Cleared");
    }

    // Draw gizmos for each position in the list.
    private void OnDrawGizmos()
    {
        if (tilemap == null || positions == null) return;

        Gizmos.color = Color.cyan; // Set the color for the gizmos.

        foreach (var position in positions)
        {
            // Convert the tile position to a world position and draw a sphere at each position.
            Vector3 worldPos = TileToWorld(position);
            Gizmos.DrawSphere(worldPos, 0.3f);
        }
    }
}
