using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioEventListener : ChannelListener<Action<AudioClip>>
{
    [SerializeField] private AudioSource soundSource;

    public override void Subscribe()
    {
        eventChannel.Subscribe(OnEventRaised);
    }

    public override void Unsubscribe()
    {
        eventChannel.Unsubscribe(OnEventRaised);
    }

    private void OnEventRaised (AudioClip clip)
    {
       if (!soundSource.isPlaying) soundSource.PlayOneShot(clip);
    }

}
