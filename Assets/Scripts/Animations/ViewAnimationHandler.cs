using UnityEngine;
using DG.Tweening;

public class ViewAnimationHandler : MonoBehaviour
{
    [SerializeField] private float animationDuration = 0.25f;
    [SerializeField] private float initialScale = 0.9f;
    [SerializeField] private float endScale = 1f;
    [SerializeField] private float finalScale = 0.6f;
    [SerializeField] private float bounceIntencity = 2f;
    private void OnEnable()
    {
        transform.localScale = Vector3.one * initialScale;

        transform.DOScale(endScale, animationDuration)
        .SetEase(Ease.OutBack, bounceIntencity)
        .From(Vector3.one * initialScale);
    }

    private void OnDisable()
    {
        transform.DOScale(finalScale, animationDuration)
            .SetEase(Ease.OutBack, bounceIntencity)
            .From(Vector3.one * endScale);
    }
}
