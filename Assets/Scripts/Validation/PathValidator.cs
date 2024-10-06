
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
            Debug.Log("Pos:"+nextPosition);
            if (tile != null)
            {
               return StepValidator.IsValid(direction,tile as Tile);
            }
            return false;
        }
    }
}
