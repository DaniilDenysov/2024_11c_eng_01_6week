using System;
using System.Collections.Generic;
using Characters;
using Characters.Skills;
using Collectibles;
using Managers;
using Mirror;
using UnityEngine;

namespace Traps
{
    public class SlimeTrail : NetworkBehaviour
    {
        private Attack _attack;
        private Inventory _collection;
        [SerializeField] private CharacterMovement _ownerMovement;
        private const string GroupName = "SnailTrail";
        private const int LiveTime = 9;
        [SyncVar] private int _liveTime;
        private const int StepsConsumes = 2;

        [ClientRpc]
        public void RpcSetUp(GameObject owner)
        {
            _liveTime = LiveTime;
            transform.position = owner.transform.position;
            
            if (owner.TryGetComponent(out _ownerMovement) 
                && owner.TryGetComponent(out _attack) 
                && owner.TryGetComponent(out _collection))
            {
                _attack.AddStaticAttackCells(new List<Vector3>{ transform.position }, GroupName);
                _collection.AddStaticPickUpCells(new List<Vector3>{ transform.position }, GroupName);
            }
            else
            {
                Debug.LogError(
                    "Gameobject making trail doesn't have Movement, Attack or Collection component");
            }

            EventManager.OnCharacterMovesOut += OnPlayerMakesMove;
            EventManager.OnTurnEnd += OnTurnEnd;
        }

        private void OnPlayerMakesMove(Vector3 cell, CharacterMovement movement)
        {
            if (movement != _ownerMovement && transform.position == movement.transform.position)
            {
                movement.DecreaseStep();
            }
        }

        private void OnTurnEnd()
        {
            if (_liveTime > 0)
            {
                _liveTime--;
            }
            else
            {
                RemoveFromField();
            }
        }

        public void RemoveFromField()
        {
            _attack.RemoveStaticAttackCells(GroupName);
            _collection.RemoveStaticPickUpCells(GroupName);
            _attack = null;
            _collection = null;
            
            EventManager.OnCharacterMovesOut -= OnPlayerMakesMove;
            EventManager.OnTurnEnd -= OnTurnEnd;
            
            Destroy(gameObject);
        }

        public bool IsTrailPositionedAt(Vector3 trail)
        {
            return transform.position.Equals(trail);
        }

        public GameObject GetOwner()
        {
            return _ownerMovement.gameObject;
        }
    }
}