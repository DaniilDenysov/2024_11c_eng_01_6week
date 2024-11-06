using Collectibles;
using CustomTools;
using Managers;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Collectibles
{
    public class Human : ICollectible
    {
        [SerializeField, SyncVar] private string ownedBy;
        [SerializeField, Range(2, 6), SyncVar] private int currentPoints;
        [SerializeField, SyncVar] private bool isCollected;
        [SerializeField] private float fearRange = 5f;
        private bool isPanicking = false;
        private Animator animator;

        [Server]
        public void SetOwner(string owner)
        {
            ownedBy = owner;
        }

        private void Awake()
        {
            Debug.Log("Human Awake started");
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Animator component is missing on Human GameObject");
            }

            FindObjectOfType<HumanReactionManager>()?.RegisterHuman(this);
        }

        private void Update()
        {
            if (isCollected && gameObject.activeInHierarchy)
            {
                gameObject.SetActive(false);
            }
            else
            {
                CheckForNearbyCharacters();
            }
        }

        public HumanDTO GetData()
        {
            return new HumanDTO()
            {
                CharacterGUID = ownedBy,
                Amount = currentPoints
            };
        }

        private void CheckForNearbyCharacters()
        {
            GameObject[] characters = GameObject.FindGameObjectsWithTag("Character");
            bool isCharacterNearby = false;

            foreach (GameObject character in characters)
            {
                float distance = Vector3.Distance(character.transform.position, transform.position);
                if (distance <= fearRange)
                {
                    isCharacterNearby = true;
                    break;
                }
            }

            if (isCharacterNearby)
            {
                EnterPanic();
            }
            else
            {
                ExitPanic();
            }
        }

        public void EnterPanic()
        {
            if (!isPanicking)
            {
                isPanicking = true;
                animator.SetTrigger("Panic");
                Debug.Log("Human is panicking!");
            }
        }

        public void ExitPanic()
        {
            if (isPanicking)
            {
                isPanicking = false;
                animator.SetTrigger("Calm");
                Debug.Log("Human is calm.");
            }
        }

        public override object Collect()
        {
            FindObjectOfType<HumanReactionManager>()?.UnregisterHuman(this);
            CmdCollect();
            return this;
        }
        
        [Command(requiresAuthority = false)]
        private void CmdCollect()
        {
            RpcSetCollected();
        }
    
        [ClientRpc]
        private void RpcSetCollected()
        {
            Destroy(gameObject);
        }
    }
}
