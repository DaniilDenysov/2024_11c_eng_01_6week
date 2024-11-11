using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio channel", menuName = "create audio channel")]
public class AudioEventChannel : EventChannel<Action<AudioClip>>
{
    public void RaiseEvent(AudioClip clip)
    {
        func?.Invoke(clip);
    }

    public override void Subscribe(Action<AudioClip> func)
    {
        this.func += func;
    }

    public override void Unsubscribe(Action<AudioClip> func)
    {
        this.func -= func;
    }
}
