using DesignPatterns.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChannelListener<T> : Singleton<ChannelListener<T>>
{
    [SerializeField] protected EventChannel<T> eventChannel;

    public override ChannelListener<T> GetInstance()
    {
        return this;
    }

    public override void Awake()
    {
        base.Awake();
        Subscribe();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Unsubscribe();
    }

    public abstract void Subscribe();
    public abstract void Unsubscribe();
}
