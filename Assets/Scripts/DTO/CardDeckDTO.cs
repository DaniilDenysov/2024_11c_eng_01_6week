using CustomTools;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cards
{
    [System.Serializable]
    public struct CardDeckDTO
    {
        public Card card;
        [Range(0,100)] public int amount;

        public CardDeckDTO(int amount)
        {
            this.amount = amount;
            card = null;
        }
    }
}
