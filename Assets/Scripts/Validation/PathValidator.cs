
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Validation
{
    public class PathValidator : MonoBehaviour
    {
        [SerializeField] private Tilemap walls;

        public bool CanMoveTo (Vector3 nextPosition, Vector3Int direction)
        {
            var tile = walls.GetTile(walls.WorldToCell(nextPosition));
            
            if (tile != null)
            {
               return StepValidator.IsValid(direction, tile as Tile);
            }
            return false;
        }

        public bool CanMoveTo (Vector3 initialPosition, Vector3 nextPosition)
        {
            var tile = walls.GetTile(walls.WorldToCell(nextPosition));
            
            Vector3Int direction = new Vector3Int(
                (int)(initialPosition.x - nextPosition.x),
                (int)(initialPosition.y - nextPosition.y),
                (int)(initialPosition.z - nextPosition.z)
            );
            
            if (tile != null)
            {
               return StepValidator.IsValid(direction, tile as Tile);
            }
            return false;
        }
    }
}
