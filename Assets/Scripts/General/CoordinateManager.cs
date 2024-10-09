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

        public static Vector3 GetAdjustedTileAdjustedPosition(Vector3 origin, Vector3 cellUnit, Vector3 position) {
            return new Vector3((float)roundByUnitGrid(origin.x, cellUnit.x, position.x), 
                (float)roundByUnitGrid(origin.y, cellUnit.y, position.y), 0);
        }

        private static float roundByUnitGrid(float offset, float unit, float number) {
            float offsetedNumber = number - offset;
            float reminder = offsetedNumber % unit;

            return number - reminder + (reminder >= (unit / 2) ? unit : 0);
        }
    }
}