using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spawns
{
    [System.Serializable]
    public struct SpawnArea
    {
        public CharacterData OwnedBy;
        public float SizeX, SizeY;
        public Vector3 Pivot;
    }
}
