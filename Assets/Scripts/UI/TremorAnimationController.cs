using General;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    [RequireComponent(typeof(Animator))]
    public class TremorAnimationController : NetworkBehaviour
    {
        private Animator _animator;
        private void Start()
        {
            _animator = GetComponent<Animator>();
            _animator.SetInteger("AnimId", GetLocalPlayerId());
        }
        
        public int GetLocalPlayerId()
        {
            foreach (var player in NetworkPlayerContainer.Instance.GetItems())
            {
                if (player.isOwned)
                {
                    switch (player.gameObject.name)
                    {
                        case "Brail": return 0;
                        case "Froje": return 1;
                        case "Horntipede": return 2;
                        case "Ishroom": return 3;
                        default: return -1;
                    }
                }
            }
            
            return -1;
        }
    }
}