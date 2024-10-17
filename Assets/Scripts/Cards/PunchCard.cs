using System;
using Characters;
using System.Collections;
using System.Collections.Generic;
using Ganeral;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;
using Validation;

namespace Cards
{
    public class PunchCard : Card
    {
        private Attack _attack;

        public override void OnCardActivation(GameObject activator)
        {
            activator.TryGetComponent(out _attack);
            
            List<Vector3> litPositions = _attack.GetAttackCells(1);

            if (litPositions.Capacity > 0)
            {
                TileSelector.Instance.SetTilesLit(litPositions, AttackCell);
            }
            else
            {
                OnCardSetUp(false);
            }
        }
        
        private void AttackCell(Vector3 chosenTile)
        {
            OnCardSetUp(_attack.TryAttack(chosenTile));
        }
    }
}
