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

        public abstract T GetInstance();
    }
}
