using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioEventListener : ChannelListener<Action<AudioClip>>
{
    [SerializeField] private bool dontDestroyOnLoad;
    private AudioSource audioSource;

    public override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
        if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
    }

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
        audioSource.PlayOneShot(clip);
    }
}
