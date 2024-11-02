using UnityEngine;

namespace Extensions.Vector
{
    public static class Vector3IntExtensions
    {

        public static Vector3Int VectorToIntVector(this Vector3 vector)
        {
            return new Vector3Int((int)vector.x, (int)vector.y, 0);
        }


        public static Vector3Int NormalizeIntVector(this Vector3Int vector)
        {
            float threshold = 1;

            return new Vector3Int(
                vector.x >= threshold ? 1 : vector.x <= -threshold ? -1 : 0,
                vector.y >= threshold ? 1 : vector.y <= -threshold ? -1 : 0,
                0
            );
        }
    }
}
