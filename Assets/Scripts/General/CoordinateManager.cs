using System;
using System.Collections.Generic;
using Characters;
using Collectibles;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Ganeral
{
    public class CoordinateManager : MonoBehaviour
    {
        public static List<GameObject> GetEntities(Vector3 position, GameObject self = null)
        {
            var circles = Physics2D.OverlapCircleAll(position, 0.1f);

            List<GameObject> result = new List<GameObject>();
            foreach (Collider2D circle in circles) {
                if (circle != default && circle.gameObject != self) {
                    result.Add(circle.gameObject);
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
        
        public static List<Vector3> GetAttackDirections() {
            return new List<Vector3> {
                new Vector3(0, 1),
                new Vector3(1, 0),
                new Vector3(0, -1),
                new Vector3(-1, 0),
                new Vector3(0, 0)
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

        public static bool AttackAndEatAtCell(Vector3 cell, Attack attack, ICollector collector)
        {
            bool result = false;
            
            foreach (GameObject entity in CoordinateManager.GetEntities(cell))
            {
                if (entity.TryGetComponent(out Inventory _))
                {
                    collector.PickUp(cell);
                    result = true;
                }
                else if (entity.TryGetComponent(out ICollectible collectible))
                {
                    if (collectible.GetType() == typeof(Human))
                    {
                        attack.TryAttack(cell);
                        result = true;
                    }
                }
            }

            return result;
        }
    }
}