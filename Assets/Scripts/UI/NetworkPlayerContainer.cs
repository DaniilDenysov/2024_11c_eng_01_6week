using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;

namespace General
{
    public class NetworkPlayerContainer : LabelContainer<NetworkPlayer, NetworkPlayerContainer>
    {
        public override NetworkPlayerContainer GetInstance()
        {
            return this;
        }
    }
}
