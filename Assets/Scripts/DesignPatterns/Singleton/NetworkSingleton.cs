using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPatterns.Singleton
{
    public abstract class NetworkSingleton<T> : NetworkBehaviour
    {
        public static T Instance;

        public virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = GetInstance();
            }
        }

        private void OnDestroy()
        {
            if (Instance as NetworkSingleton<T> == this)
            {
                Instance = default;
            }
        }

        public abstract T GetInstance();
    }
}