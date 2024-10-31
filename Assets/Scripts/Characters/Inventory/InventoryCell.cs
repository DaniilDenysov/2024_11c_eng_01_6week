using Collectibles;
using System.Collections;
using System.Collections.Generic;
using CustomTools;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inventory
{
    public class InventoryCell : MonoBehaviour
    {
        [SerializeField] private HumanDTO container;
        [SerializeField] private Image display;

        public bool TryAdd(HumanDTO human)
        {
            if (IsEmpty())
            {
                container = human;
                SetDisplayTransparency(1f);
                return true;
            }
            return false;
        }

        public bool TryRemove()
        {
            if (!IsEmpty())
            {
                container = new HumanDTO();
                SetDisplayTransparency(0f);
                return true;
            }
            return false;
        }

        private void SetDisplayTransparency (float transparency)
        {
            display.color = new Color(display.color.r, display.color.g, display.color.b, transparency);
        }

        private bool IsEmpty() => string.IsNullOrEmpty(container.CharacterGUID);
    }
}