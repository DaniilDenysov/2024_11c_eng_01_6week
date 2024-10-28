using System;
using System.Collections.Generic;
using Characters;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Validation
{
    public class PathValidator : MonoBehaviour
    {
        [SerializeField] private Tilemap walls;

        public bool CanMoveTo(Vector3 nextPosition, Vector3Int direction)
        {
            var tile = walls.GetTile(walls.WorldToCell(nextPosition));

            if (tile != null)
            {
                return StepValidator.IsValid(direction, tile as Tile);
            }
            return false;
        }

        public bool CanMoveTo(Vector3 initialPosition, Vector3 nextPosition)
        {
            Vector3 directionUnit = GetUnitDirection(walls, initialPosition, nextPosition);

            if (directionUnit.x == 0 || directionUnit.y == 0)
            {
                for (Vector3 cursor = new Vector3(initialPosition.x, initialPosition.y) 
                                                + multiplyVectors(walls.layoutGrid.cellSize, directionUnit); 
                     cursor != nextPosition + multiplyVectors(walls.layoutGrid.cellSize, directionUnit);
                     cursor += multiplyVectors(walls.layoutGrid.cellSize, directionUnit))
                {
                    if (CanMoveTo(cursor, CharacterMovement.VectorToIntVector(directionUnit * -1)) == false)
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        public bool IsOutOfMap(Vector3 cell)
        {
            return !walls.cellBounds.Contains(walls.WorldToCell(cell));
        }

        private static Vector3 multiplyVectors(Vector3 firstVector, Vector3 secondVector)
        {
            return new Vector3(firstVector.x * secondVector.x, firstVector.y * secondVector.y, firstVector.z * secondVector.z);
        }

        public List<Vector3> GetAvailableCells(Vector3 fromCell, int range)
        {
            List<Vector3> result = new List<Vector3>();
            
            List<Vector3> directions = CharacterMovement.GetAllDirections();

            foreach (Vector3 direction in directions)
            {
                for (int i = 0; i < range; i++)
                {
                    if (CanMoveTo(fromCell, fromCell + direction * (i + 1)))
                    {
                        result.Add(fromCell + direction * (i + 1));
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return result;
        }

        public static Vector3 GetUnitDirection(Tilemap tilemap, Vector3 firstPosition, Vector3 secondPosition) {
            Vector3 unitSize = tilemap.layoutGrid.cellSize;

            Vector3Int firstTilePosition = tilemap.WorldToCell(firstPosition);
            Vector3Int secondTilePosition = tilemap.WorldToCell(secondPosition);
            Vector3Int vectorDifference = secondTilePosition - firstTilePosition;

            return new Vector3(
                vectorDifference.x > 0 ? unitSize.x : (vectorDifference.x < 0 ? -unitSize.x : 0),
                vectorDifference.y > 0 ? unitSize.y : (vectorDifference.y < 0 ? -unitSize.y : 0),
                0
            );
        }
    }
}
