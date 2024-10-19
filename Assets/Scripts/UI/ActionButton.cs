using System;
using ModestTree.Util;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class ActionButton : MonoBehaviour
    {
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        public void ApplyAction(Action onAction)
        {
            _button.onClick.AddListener(() =>
            {
                onAction.Invoke();
                gameObject.SetActive(false);
            });
        }
    }
}