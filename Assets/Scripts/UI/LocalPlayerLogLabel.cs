using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace UI.PlayerLog
{
    public class LocalPlayerLogLabel : MonoBehaviour
    {
        [SerializeField] private TMP_Text display;
        [SerializeField, Range(0, 100f)] private float fadeOutTimer;

        public void Construct(string message)
        {
            display.text = message;
            StartCoroutine(Fade(fadeOutTimer));
        }

        public IEnumerator Fade(float fadeOutTimer)
        {
            Color originalColor = display.color;
            float fadeAmount = 0f;
            float delay = fadeOutTimer / 100f;

            while (fadeAmount < 1f)
            {
                fadeAmount += Time.deltaTime / fadeOutTimer;
                Color newColor = originalColor;
                newColor.a = Mathf.Lerp(1f, 0f, fadeAmount);
                display.color = newColor;
                yield return null;
            }
            display.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
            Destroy(gameObject);
        }
    }
}
