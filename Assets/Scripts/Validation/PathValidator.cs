using Ganeral;
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
            Vector3 directionUnit = CoordinateManager.GetUnitDirection(walls, initialPosition, nextPosition);

            if (directionUnit.x == 0 || directionUnit.y == 0)
            {
                for (Vector3 cursor = new Vector3(initialPosition.x, initialPosition.y) 
                                                + multiplyVectors(walls.layoutGrid.cellSize, directionUnit);
                    !CoordinateManager.IsSameCell(cursor, nextPosition + multiplyVectors(walls.layoutGrid.cellSize, directionUnit));
                    cursor += multiplyVectors(walls.layoutGrid.cellSize, directionUnit))
                {
                    if (CanMoveTo(cursor, CoordinateManager.VectorToIntVector(directionUnit * -1)) == false)
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

        public Tilemap GetTileMap()
        {
            return walls;
        }

        private static Vector3 multiplyVectors(Vector3 firstVector, Vector3 secondVector)
        {
            return new Vector3(firstVector.x * secondVector.x, firstVector.y * secondVector.y, firstVector.z * secondVector.z);
        }
    }
}
