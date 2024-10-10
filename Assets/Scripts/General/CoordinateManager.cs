using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Ganeral
{
    public class CoordinateManager : MonoBehaviour
    {
        public static List<Transform> GetEntities(Transform transform, Vector3 direction)
        {
            Vector3 origin = transform.position;

            var circles = Physics2D.OverlapCircleAll(origin + direction, 0.1f);
            Debug.DrawRay(transform.position, direction, Color.red, 10f);
            List<Transform> result = new List<Transform>();

            foreach (Collider2D circle in circles) {
                if (circle != default && circle.transform != transform) {
                    result.Add(circle.transform);
                }
            }

            return result;
        }

        public static List<Vector3> GetAllDirections() {
            return new List<Vector3> {
                new Vector3(0, 1),
                new Vector3(1, 0),
                new Vector3(0, -1),
                new Vector3(-1, 0)
            };
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

        public static bool IsSameCell(Vector3 firstPosition, Vector3 secondPosition) {
            float threshold = 0.05f;

            return Vector2.Distance(firstPosition, secondPosition) < threshold;
        }

        public static Vector3Int VectorToIntVector(Vector3 vector) {
            return new Vector3Int((int)vector.x, (int)vector.y, 0);
        }

        public static Vector3Int NormalizeIntVector(Vector3Int vector) {
            float threshold = 1;

            return new Vector3Int(
                vector.x >= threshold ? 1 : vector.x <= -threshold ? -1 : 0,
                vector.y >= threshold ? 1 : vector.y <= -threshold ? -1 : 0,
                0
            );
        }
    }
}