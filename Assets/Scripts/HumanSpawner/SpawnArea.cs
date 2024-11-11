using System;
using UnityEngine;

namespace Spawns
{
    [Serializable]
    public struct SpawnArea
    {
        public CharacterData OwnedBy;
        public float SizeX, SizeY;
        public Vector3 Pivot;

        public bool IsPositionInside(Vector3 position, Vector3 offset)
        {
            Vector2 positionNormalized = new Vector3( position.x - Pivot.x, position.y - Pivot.y) - offset;
            
            return positionNormalized.x < SizeX && positionNormalized.x > 0
                    && positionNormalized.y < SizeY && positionNormalized.y > 0;
        }
    }
}
