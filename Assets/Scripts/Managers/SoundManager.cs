using DesignPatterns.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : Singleton<SoundManager>
    {
        private AudioSource audioSource;

        public override void Awake()
        {
            base.Awake();
            audioSource = GetComponent<AudioSource>();
        }

        public override SoundManager GetInstance()
        {
            return this;
        }

        public void PlaySound (AudioClip clip)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
