using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class GlobalTickManager : Manager
    {
        [SerializeField, Range(0, 100f)] private float tickRate;

        public void Start()
        {
            InvokeRepeating(nameof(GenerateTick),tickRate, tickRate);
        }

        private void GenerateTick ()
        {
            EventManager.FireEvent(EventManager.OnTick);
        }
    }
}
