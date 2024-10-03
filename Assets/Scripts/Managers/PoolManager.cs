using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using CustomTools;
using Zenject;
using Shooting;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Managers
{
    public class PoolManager :  Manager
    {
        public static PoolManager Instance;
        private Dictionary<string, Queue<GameObject>> _pool = new Dictionary<string, Queue<GameObject>>();
        [SerializeField] private SerializedDictionary<Poolable, int> _poolableRefs = new SerializedDictionary<Poolable, int>();
        [SerializeField] private SerializedDictionary<string, Poolable> _poolables = new SerializedDictionary<string, Poolable>();

        [SerializeField] private bool instanceOnDemand = true;

        /*     public override void InstallBindings()
             {
                 PopulatePool();
                 Container.Bind<PoolManager>().FromComponentOn(gameObject).AsSingle();
             }*/


#if UNITY_EDITOR
        [Button]
        public void AutowirePrefabs()
        {
            // Load all prefabs of type Poolable in the project
            var guids = AssetDatabase.FindAssets("t:Prefab"); // Find all prefabs
            _poolables.Clear(); // Clear the dictionary before adding new entries

            foreach (var guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                if (prefab != null)
                {
                    Poolable poolable = prefab.GetComponent<Poolable>();
                    if (poolable != null)
                    {
                        // Add the prefab's name and Poolable instance to the dictionary
                        _poolables[prefab.name] = poolable;
                        if (_poolableRefs.TryAdd(poolable, 0)) _poolableRefs[poolable] = 0;
                    }
                }
            }

            // Save changes made to the serialized dictionary
            EditorUtility.SetDirty(this);
        }
#endif

        [Button]
        public void PopulatePool()
        {
            foreach (var poolable in _poolableRefs)
            {
                // Ensure that the pool contains a Queue for each poolable
                if (!_pool.ContainsKey(poolable.Key.GetPoolableName()))
                {
                    _pool[poolable.Key.GetPoolableName()] = new Queue<GameObject>();
                }

                var poolQueue = _pool[poolable.Key.GetPoolableName()];

                // Instantiate the objects and enqueue them
                for (int i = 0; i < poolable.Value; i++)
                {
                    var newInstance = Instantiate(poolable.Key).gameObject;
                    newInstance.SetActive(false);
                    poolQueue.Enqueue(newInstance);
                }
            }
        }

        public GameObject Get(string poolableName)
        {
            if (_pool.TryGetValue(poolableName, out Queue<GameObject> poolQueue))
            {
                if (poolQueue.Count > 0)
                {
                    var obj = poolQueue.Dequeue();  // Take the object from the front of the queue
                    obj.SetActive(true);
                    return obj;
                }
            }
            if (instanceOnDemand)
            {
                // Instantiate a new object if the pool is empty
                Poolable tmp = Instantiate(_poolables[poolableName], Vector3.zero, Quaternion.identity);

                if (tmp.gameObject != null)
                {
                    Put(poolableName, tmp.gameObject);  // Add the new object to the pool
                    return tmp.gameObject;
                }
            }
            Debug.LogError($"Object {poolableName} hasn't been found in pool register");
            return null;
        }

        public void PutAll()
        {
            foreach (KeyValuePair<string, Queue<GameObject>> pair in _pool)
            {
                foreach (var obj in pair.Value)
                {
                    if (obj.activeInHierarchy)
                    {
                        obj.SetActive(false);
                    }
                }
            }
        }

        public void Put(string poolableName, GameObject obj)
        {
            if (_pool.TryGetValue(poolableName, out Queue<GameObject> poolQueue))
            {
                obj.SetActive(false);
                poolQueue.Enqueue(obj);  // Add the object back to the queue
            }
            else
            {
                Queue<GameObject> newQueue = new Queue<GameObject>();
                obj.SetActive(false);
                newQueue.Enqueue(obj);
                _pool[poolableName] = newQueue;  // Create a new queue if one doesn't exist
            }
        }

        private void OnDestroy()
        {
            _pool.Clear();
        }
    }
}