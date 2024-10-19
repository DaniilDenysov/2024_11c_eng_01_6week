using System;
using System.Collections.Generic;
using System.Linq;
using Characters;
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
    private List<Vector3> _litPositions;

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
                if (_litPositions.Contains(tilePosition + cellUnit))
                {
                    SetTilesUnlit();
                    _onChosen.Invoke(tilePosition + cellUnit);
                }
            }
        }
    }

    public void SetTilesLit(List<Vector3> positions, Action<Vector3> onChosen)
    {
        if (positions.Count < 1)
        {
            Debug.LogError("Lit cells count is 0");
        }
        
        SetTilesUnlit();
        foreach (Vector3 position in positions)
        {
            Vector3Int tilePosition = tileMap.WorldToCell(position);
            tileMap.SetTile(tilePosition, highlightTile);
        }

        _onChosen = onChosen;
        _litPositions = positions;
    }

    public void SetDirectionsTilesLit(Vector3 position, Action<Vector3> onChosen, 
        List<Vector3> excludeDirections = null)
    {
        List<Vector3> directionPositions = CharacterMovement.GetAllDirections();

        for (int i = 0; i < directionPositions.Count; i++)
        {
            if (excludeDirections == null || !excludeDirections.Contains(directionPositions[i]))
            {
                directionPositions[i] += position;
            }
            else
            {
                directionPositions.RemoveAt(i);
                i--;
            }
        }

        SetTilesLit(directionPositions, onChosen);
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