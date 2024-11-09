using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Collectibles
{
    [System.Serializable]
    public struct HumanDTO
    {
        public string CharacterGUID;
        public int Amount;

        public HumanDTO Copy()
        {
            return new HumanDTO()
            {
                CharacterGUID = CharacterGUID,
                Amount = Amount
            };
        }
    }
}
