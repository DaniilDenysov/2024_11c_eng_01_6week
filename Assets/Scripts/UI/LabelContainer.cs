using DesignPatterns.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace UI
{
    public abstract class LabelContainer<I,T> : Singleton<T> where I : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] protected Transform container;
        [SerializeField] protected List<I> items = new List<I>();


        public virtual void Add (I item)
        {
            item.transform.SetParent(container);
            items.Add(item);
        }

        public virtual List<I> GetItems ()
        {
            items.RemoveAll((i)=>i==null);
            return items;
        }

    }
}
