using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class DiceManager : Manager
    {
        public int GetDiceValue ()
        {
            return Random.Range(1,7);
        }
    }
}
