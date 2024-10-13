using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using ModestTree;
using Traps;
using UnityEngine;
using UnityEngine.Serialization;

namespace Characters.Skills
{
    public class Trail : Skill
    {
        [SerializeField] private SlimeTrail trail;
        private List<SlimeTrail> _currentTrail;
        private const int MaxTrailSize = 5;

        private void Awake()
        {
            _currentTrail = new List<SlimeTrail>();
        }

        public override void Activate(Action<bool> onSetUp)
        {
            base.Activate(onSetUp);
            clearTrail();
            
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
            
            SlimeTrail newTrail = Instantiate(trail, newPosition, transform.rotation);
            newTrail.SetUp(gameObject);
            _currentTrail.Add(newTrail);
        }

        public override bool IsActivatable()
        {
            return true;
        }

        public void clearTrail()
        {
            while (!_currentTrail.IsEmpty())
            {
                _currentTrail[0].RemoveFromField();
                _currentTrail.RemoveAt(0);
            }
        }
    }
}