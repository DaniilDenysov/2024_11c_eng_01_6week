using System;
using System.Collections.Generic;
using Characters;
using Mirror;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSelector : MonoBehaviour
{
    public static TileSelector Instance;
    [SerializeField] private Tilemap tileMap;
    private Vector3 cellUnit;
    [SerializeField] private TileBase highlightTile;
    [SerializeField] private TileBase arrowTile;
    
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

            TileClicked(tilePosition);
        }
    }

    public void TileClicked(Vector3Int tilePosition)
    {
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

    public void SetTilesLit(List<Vector3> positions, Action<Vector3> onChosen, 
        TileBase tile = null, List<Vector3> directions = null)
    {
        if (positions.Count < 1)
        {
            Debug.LogError("Lit cells count is 0");
        }

        if (directions != null)
        {
            if (directions.Count != positions.Count)
            {
                Debug.Log("Direction Count doesn't equal to position count");
                return;
            }
        }

        SetTilesUnlit();
        for (int i = 0; i < positions.Count; i++) 
        {
            Vector3Int tilePosition = tileMap.WorldToCell(positions[i]);
            tileMap.SetTile(tilePosition, tile != null ? tile : highlightTile);

            if (directions != null)
            {
                float angle = Vector3.Angle(directions[i], transform.up);
                
                if (directions[i].x > 0)
                {
                    angle = -angle;
                }
                
                tileMap.SetTransformMatrix(tilePosition, Matrix4x4.TRS(Vector3.zero, 
                    Quaternion.Euler(0, 0, angle), Vector3.one));
            }
        }

        _onChosen = onChosen;
        _litPositions = positions;
    }

    public void SetDirectionsTilesLit(Vector3 position, Action<Vector3> onChosen, 
        List<Vector3> excludeDirections = null)
    {
        List<Vector3> directionPositions = CharacterMovement.GetAllDirections();
        List<Vector3> directions = new List<Vector3>();

        for (int i = 0; i < directionPositions.Count; i++)
        {
            if (excludeDirections == null || !excludeDirections.Contains(directionPositions[i]))
            {
                directions.Add(directionPositions[i]);
                directionPositions[i] += position;
            }
            else
            {
                directionPositions.RemoveAt(i);
                i--;
            }
        }

        SetTilesLit(directionPositions, onChosen, arrowTile, directions);
    }
    
    public void SetTilesUnlit()
    {
        if (tileMap != null) tileMap.ClearAllTiles();
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