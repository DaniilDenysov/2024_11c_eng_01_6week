using Characters;
using System.Collections.Generic;
using UnityEngine;

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
                HighlightDrawer.Instance.HighlightCells(litPositions);
                InputManager.Instance.AddCellCallbacks(new HashSet<Vector3>(litPositions),AttackCell);
            }
            else
            {
                Cancel();
            }
        }

        public override bool SingletonUse()
        {
            return true;
        }

        private void AttackCell(Vector3 chosenTile)
        {
            OnCardSetUp(_attack.TryAttack(chosenTile));
        }
    }
}
