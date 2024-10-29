using CustomTools;
using UnityEditor;
using UnityEngine;

namespace Cards
{
    [System.Serializable]
    public struct CardDeckDTO
    {
        public Card Card;
        [Range(0,100)] public int Amount;
    }
}
