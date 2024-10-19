using Mirror;
using Spawns.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using AYellowpaper.SerializedCollections;
using General;
using System.Linq;

namespace Spawns
{
    public class HumanSpawner : NetworkBehaviour
    {
        [SerializeField] private SerializedDictionary<SpawnArea, List<SpawnData>> datas = new SerializedDictionary<SpawnArea, List<SpawnData>>();

        [Header("Grid")]
        [SerializeField] private Tilemap grid;

        [Server]
        private void Start()
        {
            Spawn();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            foreach (var area in datas.Keys)
            {
                var size = new Vector3(area.SizeX, area.SizeY, 0f);
                Vector3 position = transform.position + area.Pivot + (size / 2);
                Gizmos.DrawWireCube(position, size);
            }
        }
#endif

        [Server]
        public void Spawn()
        {
            HashSet<Vector3> usedCells = new HashSet<Vector3>();
            foreach (var entry in datas)
            {
                var area = entry.Key;
                var spawnDataList = entry.Value;

                foreach (var data in spawnDataList)
                {
                    for (int i = 0; i < data.Amount;)
                    {
                        float posX = Random.Range(area.Pivot.x, area.Pivot.x + area.SizeX);
                        float posY = Random.Range(area.Pivot.y, area.Pivot.y + area.SizeY);
                        Vector3 spawnPosition = new Vector3(posX, posY, 0f) + transform.position;
                        Vector3Int tilePosition = grid.WorldToCell(spawnPosition);
                        if (usedCells.Contains(tilePosition)) continue;
                        if (NetworkPlayerContainer.Instance.GetItems().Find((p) => usedCells.Contains(grid.WorldToCell(p.gameObject.transform.position))) != null) continue;
                        usedCells.Add(tilePosition);
                        GameObject spawnedObject = Instantiate(data.Prefab, tilePosition + new Vector3(0.5f, 0.5f), Quaternion.identity);
                        NetworkServer.Spawn(spawnedObject);
                        i += 1;
                    }
                }
            }
        }
    }
}
