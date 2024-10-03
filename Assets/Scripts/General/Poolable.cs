using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Managers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Zenject;

namespace Shooting
{
    public class Poolable : MonoBehaviour
    {
        [SerializeField] protected UnityEvent _onPool;


        public virtual string GetPoolableName() => gameObject.name;

        public virtual void GetFromPool(string poolableName)
        {
            GameObject tmp = PoolManager.Instance.Get(poolableName);
            tmp.transform.position = transform.position;
            tmp.SetActive(true);
        }

        public virtual GameObject GetFromPoolWithCallback(string poolableName)
        {
            GameObject tmp = PoolManager.Instance.Get(poolableName);
            tmp.transform.position = transform.position;
            tmp.SetActive(true);
            return tmp;
        }

        public virtual void Pool()
        {
            PoolManager.Instance.Put(GetPoolableName(), gameObject);
            _onPool.Invoke();
            gameObject.SetActive(false);
        }

        public void Invoke(Vector2 position)
        {
            GameObject obj = PoolManager.Instance.Get(gameObject.name);
            obj.transform.position = position;
        }
    }
}