using System;
using System.Collections.Generic;
using Characters;
using Characters.Skills;
using Collectibles;
using Ganeral;
using Managers;
using UnityEngine;

namespace Traps
{
    public class SlimeTrail : MonoBehaviour
    {
        private Attack _attack;
        private ICollector _collection;
        private CharacterMovement _ownerMovement;
        private const string GroupName = "SnailTrail";
        private const int LiveTime = 9;
        private int _liveTime;

        public void SetUp(GameObject owner)
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

            foreach (GameObject entity in CoordinateManager.GetEntities(transform.position))
            {
                if (entity.TryGetComponent(out CharacterMovement movement))
                {
                    if (movement != _ownerMovement)
                    {
                        movement.SetStepCost(2);
                    }
                }
            }

            EventManager.OnCharacterMovesIn += OnPlayerMakesMoveIn;
            EventManager.OnCharacterMovesOut += OnPlayerMakesMoveOut;
            EventManager.OnTurnEnd += OnTurnEnd;
        }

        private void OnPlayerMakesMoveIn(Vector3 cell, CharacterMovement movement)
        {
            if (movement != _ownerMovement && transform.position == movement.transform.position)
            {
                movement.SetStepCost(2);
            }
        }
        
        private void OnPlayerMakesMoveOut(Vector3 cell, CharacterMovement movement)
        {
            if (movement != _ownerMovement && transform.position == movement.transform.position)
            {
                movement.ResetStepCost();
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
            
            EventManager.OnCharacterMovesIn -= OnPlayerMakesMoveIn;
            EventManager.OnCharacterMovesOut -= OnPlayerMakesMoveOut;
            EventManager.OnTurnEnd -= OnTurnEnd;
        }

        public bool IsTrailPositionedAt(Vector3 trail)
        {
            return transform.position.Equals(trail);
        }
    }
}