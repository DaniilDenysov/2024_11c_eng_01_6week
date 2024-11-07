using DesignPatterns.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace UI
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="I">item</typeparam>
    /// <typeparam name="T">self</typeparam>
    public abstract class LabelContainer<I,T> : Singleton<T> where I : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] protected Transform container;
        [SerializeField] protected List<I> items = new List<I>();

        public virtual void Add (I item)
        {
            item.transform.SetParent(container);
            items.Add(item);
        }

        public List<I> GetItems ()
        {
            items.RemoveAll((i)=>i==null);
            return items;
        }
        
        public I Remove(I item)
        {
            item.transform.SetParent(null);
            items.Remove(item);
            return item;
        }

        public int GetAmount()
        {
            return items.Count;
        }
    }
}
