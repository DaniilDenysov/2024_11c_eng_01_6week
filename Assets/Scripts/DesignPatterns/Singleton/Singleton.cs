using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPatterns.Singleton
{
    public abstract class Singleton<T> : MonoBehaviour
    {
        public static T Instance;

        public virtual void Awake ()
        {
            if (Instance == null)
            {
                Instance = GetInstance();
            }
        }

        private void OnDestroy()
        {
            if (Instance as Singleton<T> == this)
            {
                Instance = default;
            }
        }

        public abstract T GetInstance();
    }
}
