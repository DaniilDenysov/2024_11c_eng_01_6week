using Mirror;
using System.Linq;
using DesignPatterns.Singleton;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class AudioSyncManager : NetworkSingleton<AudioSyncManager>
    {
        [SerializeField] private List<AudioClip> registeredClips = new List<AudioClip>();
        [SerializeField] private AudioEventChannel inputEventChannel, outputEventChannel;

        public override void Awake()
        {
            base.Awake();
            inputEventChannel.Subscribe(TryPlayClip);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            inputEventChannel.Unsubscribe(TryPlayClip);
        }

        public override AudioSyncManager GetInstance()
        {
            return this;
        }

        public void TryPlayClip(AudioClip clip)
        {
            if (!NetworkServer.active && !NetworkClient.active) return;
            int index = registeredClips.IndexOf(clip);
            if (index == -1)
            {
                Debug.LogWarning("Clip not registered in AudioSyncManager.");
                return;
            }

            PlaySound(index);
        }

        [ClientRpc(includeOwner = false)]
        private void PlaySound(int id)
        {
           outputEventChannel.RaiseEvent(registeredClips[id]);
        }
    }
}
