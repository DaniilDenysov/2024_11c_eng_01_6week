using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class GlobalTickManager : Manager
    {
        [SerializeField, Range(0, 100f)] private float tickRate;

        public override void Start()
        {
            base.Start();
            InvokeRepeating(nameof(GenerateTick),tickRate, tickRate);
        }

        private void GenerateTick ()
        {
            EventManager.FireEvent(EventManager.OnTick);
        }

        public override void InstallBindings()
        {
            Container.Bind<GlobalTickManager>().FromInstance(this).AsSingle();
        }
    }
}
