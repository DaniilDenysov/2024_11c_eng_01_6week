using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Mirror;
using ModestTree;
using Traps;
using UnityEngine;
using UnityEngine.Serialization;

namespace Characters.Skills
{
    public class Trail : Skill
    {
        [SerializeField] private SlimeTrail trail;
        private readonly SyncList<SlimeTrail> _currentTrail = new ();
        private const int MaxTrailSize = 5;

        public override void Activate(Action<bool> onSetUp)
        {
            base.Activate(onSetUp);
            ClearTrail();
            
            EventManager.OnTurnEnd += OnTurnEnd;
            EventManager.OnCharacterMovesIn += OnMoveMade;

            PlaceTrail();
            OnActivated();
        }

        public void OnTurnEnd()
        {
            EventManager.OnTurnEnd -= OnTurnEnd;
            EventManager.OnCharacterMovesIn -= OnMoveMade;
        }

        public void OnMoveMade(Vector3 cell, CharacterMovement movement)
        {
            if (movement.gameObject == gameObject)
            {
                PlaceTrail();
            }
        }

        private void PlaceTrail()
        {
            Vector3 newPosition = transform.position;
            
            foreach (SlimeTrail trail in _currentTrail)
            {
                if (trail.IsTrailPositionedAt(newPosition))
                {
                    _currentTrail.Remove(trail);
                    _currentTrail.Add(trail);
                    return;
                }
            }
            
            if (_currentTrail.Count >= MaxTrailSize)
            {
                _currentTrail[0].RemoveFromField();
                _currentTrail.RemoveAt(0);
            }

            CmdSpawnTrail(newPosition);
        }

        [Command(requiresAuthority = false)]
        private void CmdSpawnTrail(Vector3 position)
        {
            SlimeTrail newTrail = Instantiate(trail, position, transform.rotation);
            newTrail.RpcSetUp(gameObject);
            _currentTrail.Add(newTrail);
            
            NetworkServer.Spawn(newTrail.gameObject);
        }

        public override bool IsActivatable()
        {
            return true;
        }

        private void ClearTrail()
        {
            while (!_currentTrail.IsEmpty())
            {
                _currentTrail[0].RemoveFromField();
                _currentTrail.RemoveAt(0);
            }
        }
    }
}