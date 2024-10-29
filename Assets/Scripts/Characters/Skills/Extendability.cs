using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Traps;
using UnityEngine;

namespace Characters.Skills
{
    [RequireComponent(typeof(CharacterMovement))]
    public class Extendability : Skill
    {
        [SerializeField] private HorntipedeBody _bodyPrefab;
        
        private CharacterMovement _movement;
        
        private  readonly SyncList<HorntipedeBody> _body = new ();
        private const int BodyLength = 3;
        private const int AvailableTurns = 1;
        private List<List<Vector3>> _paths;

        private void Awake()
        {
            _movement = GetComponent<CharacterMovement>();
        }

        public override void Activate(Action<bool> onSetUp)
        {
            base.Activate(onSetUp);
            
            _paths = GetPaths();
            
            HighlightAvailableTiles(_paths);
        }

        private void BuildBody(Vector3 cell)
        {
            CmdSpawnBody(cell);
        }

        [Command(requiresAuthority = false)]
        private void CmdSpawnBody(Vector3 position)
        {
            RpcSpawnBody(position);
        }

        [TargetRpc]
        private void RpcSpawnBody(Vector3 position)
        {
            HorntipedeBody body = Instantiate(_bodyPrefab);
            _body.Add(body);
            body.SetUp(position, gameObject);
            NetworkServer.Spawn(body.gameObject);
            
            _paths.RemoveAll(list =>
            {
                Vector3 previousDirection;
                
                if (_body.Count() > 1)
                {
                    previousDirection = _body[^1].transform.position - _body[^2].transform.position;
                }
                else
                {
                    previousDirection = _body[^1].transform.position - transform.position;
                }
                
                return list[_body.Count() - 1] != previousDirection;
            });
            
            if (_body.Count < BodyLength)
            {
                HighlightAvailableTiles(_paths);
            }
            else
            {
                transform.position = _body.Last().transform.position;
                ClearBody();
                OnActivated();
            }
        }
        
        public override bool IsActivatable()
        {
            List<List<Vector3>> paths = new List<List<Vector3>>();
            paths.Add(new List<Vector3>());
            
            List<Vector3> traversedPositions = new List<Vector3>();
            List<Vector3> traversedDirections = new List<Vector3>();
            List<int> turnsMade = new List<int>();
            
            traversedPositions.Add(transform.position);
            traversedDirections.Add(new Vector3(0, 0));
            turnsMade.Add(0);

            for (int i = 0; i < BodyLength; i++)
            {
                List<Vector3> nextTraversedPositions = new List<Vector3>();
                List<Vector3> nextTraversedDirections = new List<Vector3>();
                List<int> nextTurnsMade = new List<int>();

                for (int i2 = 0; i2 < traversedPositions.Count; i2++)
                {
                    List<Vector3> availableTurns = new List<Vector3>();

                    if (turnsMade[i2] < AvailableTurns + 1)
                    {
                        Vector3 oppositeDirection = traversedDirections[i2] * -1;

                        availableTurns = CharacterMovement.GetAllDirections();
                        availableTurns.Remove(oppositeDirection);
                    }
                    else
                    {
                        availableTurns.Add(traversedDirections[i2]);
                    }

                    foreach (Vector3 direction in availableTurns)
                    {
                        if (_movement.GetPathValidator().CanMoveTo(traversedPositions[i2],
                                CharacterMovement.VectorToIntVector(direction)))
                        {
                            int currentTurnsMade = turnsMade[i2];

                            if (direction != traversedDirections[i2])
                            {
                                currentTurnsMade++;
                            }

                            if (i == BodyLength - 1)
                            {
                                return true;
                            }

                            nextTraversedPositions.Add(traversedPositions[i2] + direction);
                            nextTraversedDirections.Add(direction);
                            nextTurnsMade.Add(currentTurnsMade);
                        }
                    }
                }
                
                traversedPositions = nextTraversedPositions;
                traversedDirections = nextTraversedDirections;
                turnsMade = nextTurnsMade;
            }

            return false;
        }

        public List<List<Vector3>> GetPaths()
        {
            List<List<Vector3>> paths = new List<List<Vector3>>();
            paths.Add(new List<Vector3>());
            
            List<Vector3> traversedPositions = new List<Vector3>();
            List<Vector3> traversedDirections = new List<Vector3>();
            List<int> turnsMade = new List<int>();
            
            traversedPositions.Add(transform.position);
            traversedDirections.Add(new Vector3(0, 0));
            turnsMade.Add(0);

            for (int i = 0; i < BodyLength; i++)
            {
                List<List<Vector3>> nextPaths = new List<List<Vector3>>();

                List<Vector3> nextTraversedPositions = new List<Vector3>();
                List<Vector3> nextTraversedDirections = new List<Vector3>();
                List<int> nextTurnsMade = new List<int>();

                for (int i2 = 0; i2 < traversedPositions.Count; i2++)
                {
                    List<Vector3> availableTurns = new List<Vector3>();

                    if (turnsMade[i2] < AvailableTurns + 1)
                    {
                        Vector3 oppositeDirection = traversedDirections[i2] * -1;

                        availableTurns = CharacterMovement.GetAllDirections();
                        availableTurns.Remove(oppositeDirection);
                    }
                    else
                    {
                        availableTurns.Add(traversedDirections[i2]);
                    }

                    foreach (Vector3 direction in availableTurns)
                    {
                        if (_movement.GetPathValidator().CanMoveTo(traversedPositions[i2],
                                CharacterMovement.VectorToIntVector(direction)))
                        {
                            int currentTurnsMade = turnsMade[i2];

                            if (direction != traversedDirections[i2])
                            {
                                currentTurnsMade++;
                            }

                            nextPaths.Add(new List<Vector3>(paths[i2]));
                            nextPaths.Last().Add(direction);

                            nextTraversedPositions.Add(traversedPositions[i2] + direction);
                            nextTraversedDirections.Add(direction);
                            nextTurnsMade.Add(currentTurnsMade);
                        }
                    }
                }

                paths = nextPaths;
                traversedPositions = nextTraversedPositions;
                traversedDirections = nextTraversedDirections;
                turnsMade = nextTurnsMade;
            }

            return paths;
        }
        
        private void ClearBody()
        {
            foreach (HorntipedeBody bodyUnit in _body)
            {
                bodyUnit.CmdRemoveFromField();
            }
            _body.Clear();
        }
        
        public void HighlightAvailableTiles(List<List<Vector3>> paths)
        {
            List<Vector3> litDirections = new List<Vector3>();

            foreach (List<Vector3> directionList in paths)
            {
                Vector3 direction = directionList[_body.Count];
                
                if (!litDirections.Contains(direction))
                {
                    litDirections.Add(direction);
                }
            }
            
            Vector3 currentCell = _body.Count > 0 ? _body.Last().transform.position : transform.position;
            List<Vector3> litPositions = new List<Vector3>();
            
            foreach (Vector3 litDirection in litDirections)
            {
                litPositions.Add(currentCell + litDirection);
            }
            
            TileSelector.Instance.SetTilesLit(litPositions, BuildBody);
        }
    }
}