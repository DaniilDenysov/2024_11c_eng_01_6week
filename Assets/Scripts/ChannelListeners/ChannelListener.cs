using DesignPatterns.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChannelListener<T> : MonoBehaviour
{
    [SerializeField] protected EventChannel<T> eventChannel;

    public void Awake()
    {
        Subscribe();
    }

    public void OnDestroy()
    {
        Unsubscribe();
    }

    public abstract void Subscribe();
    public abstract void Unsubscribe();
}
