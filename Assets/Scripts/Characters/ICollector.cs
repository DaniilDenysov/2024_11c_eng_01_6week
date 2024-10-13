using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Collectibles
{
    public interface ICollector
    {
        bool PickUp();
        bool PickUp(Vector3 cell);
    }
}
