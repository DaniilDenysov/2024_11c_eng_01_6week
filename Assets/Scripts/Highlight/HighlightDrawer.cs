using DesignPatterns.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HighlightDrawer : Singleton<HighlightDrawer>
{
    [SerializeField] private TileBase highlightTile;
    [SerializeField] private Tilemap tilemap;

    public override void Awake()
    {
        base.Awake();

    }

    public override HighlightDrawer GetInstance()
    {
        return this;
    }

    public void HighlightCells (List<Vector3> tilePositions)
    {
        foreach (var tilePosition in tilePositions)
        {
            HighlightCell(tilePosition);
        }
    }

    public void HighlightCell (Vector3 tilePosition)
    {
        tilemap.SetTile(tilemap.WorldToCell(tilePosition), highlightTile);
    }

    public void ClearHighlightedCells ()
    {
        tilemap.ClearAllTiles();
    }
}
