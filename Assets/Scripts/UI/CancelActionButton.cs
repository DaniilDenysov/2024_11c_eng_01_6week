using System;
using ModestTree.Util;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class CancelActionButton : MonoBehaviour
    {
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        public void ApplyCancelAction(Action onCancel)
        {
            _button.onClick.AddListener(() =>
            {
                onCancel.Invoke();
                gameObject.SetActive(false);
            });
        }
    }
}