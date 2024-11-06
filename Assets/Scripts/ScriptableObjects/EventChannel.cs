using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventChannel<T> : ScriptableObject
{
    protected T func;
    public abstract void Subscribe(T func);
    public abstract void Unsubscribe(T func);
}
