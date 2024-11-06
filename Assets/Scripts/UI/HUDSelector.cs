using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HUDSelector : MonoBehaviour
    {
        [SerializeField] private List<Button> buttons;

        public void SetButtonTexts(String[] headers)
        {
            if (headers.Length != buttons.Capacity)
            {
                Debug.Log("Headers and texts are not equal");
            }
            
            for (int i = 0; i < headers.Length && i < buttons.Capacity; i++)
            {
                buttons[i].GetComponentInChildren<TMP_Text>().text = headers[i];
            }
        }
    }
}