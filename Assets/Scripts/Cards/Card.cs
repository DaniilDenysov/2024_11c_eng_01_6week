using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cards
{
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public abstract class Card<T> : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private CanvasGroup canvasGroup;
        private Transform container;
        private Camera camera;
        private RectTransform rectTransform;
        private Vector2 startPosition;
        private bool IsCardSettingUp;

        public virtual void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            container = transform.parent;
            rectTransform = GetComponent<RectTransform>();
            camera = Camera.main;
            IsCardSettingUp = false;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            canvasGroup.alpha = 0.7f;
            startPosition = transform.position;
            transform.parent = transform.parent.parent;
        }

        public void OnDrag(PointerEventData eventData)
        {
            rectTransform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            canvasGroup.alpha = 1f;
            Vector3 destination = camera.ScreenToWorldPoint(transform.position);

            var result = Physics2D.OverlapCircle(destination, 1f);
            //or use raycast
            if (result != null && result.TryGetComponent(out T action) && !IsCardSettingUp)
            {
                IsCardSettingUp = true;
                OnCardActivation(action);
            }


            transform.position = startPosition;
            transform.parent = container;
        }

        public abstract void OnCardActivation(T arg1);

        public void OnCardSetUp(bool succesfully)
        {
            if (succesfully)
            {
                Destroy(gameObject);
            }

            IsCardSettingUp = false;
        }
    }
}
