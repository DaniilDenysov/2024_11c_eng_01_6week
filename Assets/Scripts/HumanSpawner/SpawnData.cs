using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spawns.Data
{
    [System.Serializable]
    public struct SpawnData
    {
        [Range(0, 100f)] public int Amount;
        [Range(0, 100f)] public float Probability;
        public GameObject Prefab;
    }
}
