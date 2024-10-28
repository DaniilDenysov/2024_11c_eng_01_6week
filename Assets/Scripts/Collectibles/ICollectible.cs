using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Collectibles
{
    public abstract class ICollectible : NetworkBehaviour
    {
        public abstract object Collect();
    }
}
