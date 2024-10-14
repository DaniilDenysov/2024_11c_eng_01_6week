using System;
using System.Collections.Generic;
using Ganeral;
using Managers;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSelector : MonoBehaviour
{
    public static TileSelector Instance;
    [SerializeField] private Tilemap tileMap;
    private Vector3 cellUnit;
    [SerializeField] private TileBase highlightTile;
    private Action<Vector3> _onChosen;

    void Awake()
    {
        cellUnit = tileMap.layoutGrid.cellSize / 2;
        Instance = this;
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
                _onChosen.Invoke(tilePosition + cellUnit);
                SetTilesUnlit();
            }
        }
    }

    public void SetTilesLit(List<Vector3> positions, Action<Vector3> onChosen)
    {
        if (positions.Count < 1)
        {
            Debug.LogError("Lit cells count is 0");
        }
        
        foreach (Vector3 position in positions)
        {
            Vector3Int tilePosition = tileMap.WorldToCell(position);
            tileMap.SetTile(tilePosition, highlightTile);
        }

        _onChosen = onChosen;
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