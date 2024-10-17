using System.Collections.Generic;
using Ganeral;
using Managers;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSelector : MonoBehaviour
{
    [SerializeField] private Tilemap tileMap;
    private Vector3 cellUnit;
    [SerializeField] private TileBase highlightTile;

    void Awake()
    {
        cellUnit = tileMap.layoutGrid.cellSize / 2;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var tilePosition = tileMap.WorldToCell(worldPoint);
            var tile = tileMap.GetTile(tilePosition);

            if (tile)
            {
                EventManager.OnLitTileClick(tilePosition + cellUnit);

                SetTilesUnlit();
            }
        }
    }

    public void SetTilesLit(List<Vector3> positions)
    {
        foreach (Vector3 position in positions)
        {
            Vector3Int tilePosition = tileMap.WorldToCell(position);
            tileMap.SetTile(tilePosition, highlightTile);
        }
    }

    public void SetTilesLit(Vector3 originPosition, int quantity, Vector3 direction, bool includeInitTile = false)
    {
        Vector3Int originTilePosition = tileMap.WorldToCell(originPosition);
        Vector2 directionNormalized = NormalizeDirection(direction);

        for (int i = includeInitTile ? 0 : 1; i < quantity; i++)
        {
            Vector3Int tilePosition = originTilePosition +
                new Vector3Int((int)directionNormalized.x, (int)directionNormalized.y, 0) * i;
            tileMap.SetTile(tilePosition, highlightTile);
        }
    }

    public void SetTilesUnlit()
    {
        tileMap.ClearAllTiles();
    }

    public Vector2 NormalizeDirection(Vector3 direction)
    {
        Vector3 directionNormalized = direction.normalized;

        if (directionNormalized.y > 0.5)
        {
            return new Vector2(0, 1);
        }
        else if (directionNormalized.x > 0.5)
        {
            return new Vector2(1, 0);
        }
        else if (directionNormalized.y < -0.5)
        {
            return new Vector2(0, -1);
        }
        else if (directionNormalized.x < -0.5)
        {
            return new Vector2(-1, 0);
        }

        return new Vector2(0, 0);
    }
}