using System;
using System.Collections.Generic;
using Managers;
using Mirror;
using ModestTree;
using Traps;
using UnityEngine;

namespace Characters.Skills
{
    public class Trail : Skill
    {
        [SerializeField] private SlimeTrail trail;
        private readonly List<SlimeTrail> _currentTrail = new ();
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
                _currentTrail[0].CmdRemoveFromField();
                _currentTrail.RemoveAt(0);
            }

            CmdSpawnTrail(newPosition);
        }

        [Command(requiresAuthority = false)]
        private void CmdSpawnTrail(Vector3 position)
        {
            SlimeTrail newTrail = Instantiate(trail, position, transform.rotation);
            NetworkServer.Spawn(newTrail.gameObject);
            newTrail.RpcSetUp(gameObject);
            RpcSetUpSlime(newTrail);
        }
        
        [TargetRpc]
        private void RpcSetUpSlime(SlimeTrail trail)
        {
            _currentTrail.Add(trail);
        }

        public override bool IsActivatable()
        {
            return true;
        }

        private void ClearTrail()
        {
            while (!_currentTrail.IsEmpty())
            {
                _currentTrail[0].CmdRemoveFromField();
                _currentTrail.RemoveAt(0);
            }
        }
    }
}