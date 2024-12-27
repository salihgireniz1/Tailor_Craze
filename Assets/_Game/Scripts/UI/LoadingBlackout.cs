using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoadingBlackout : MonoBehaviour
{
    CanvasGroup _cg;
    CanvasGroup cg => _cg ?? (_cg = GetComponent<CanvasGroup>());

    [SerializeField] Image backgroundImage;
    [SerializeField] float fadeTime = 0.5f;

    void Awake()
    {
        cg.alpha = 1f;
    }

    public void FadeIn(UnityAction onComplete = null, bool isInstant = false)
    {
        if (isInstant)
        {
            cg.alpha = 1f;
            /* var color = backgroundImage.color;
            color.a = 1f;
            backgroundImage.color = color; */
            onComplete?.Invoke();
        }
        else
        {
            cg.DOFade(1f, fadeTime)
                .SetEase(Ease.Linear)
                .OnComplete(() => onComplete?.Invoke());
            /* backgroundImage.DOFade(1f, fadeTime)
                .SetEase(Ease.Linear)
                .OnComplete(() => onComplete?.Invoke()); */
        }
    }

    public void FadeOut(UnityAction onComplete = null, bool isInstant = false)
    {
        if (isInstant)
        {
            cg.alpha = 0f;
            /* var color = backgroundImage.color;
            color.a = 0f;
            backgroundImage.color = color; */
            onComplete?.Invoke();
        }
        else
        {
            cg.DOFade(0f, fadeTime)
                .SetEase(Ease.Linear)
                .OnComplete(() => onComplete?.Invoke());
            /* backgroundImage.DOFade(0f, fadeTime)
                .SetEase(Ease.Linear)
                .OnComplete(() => onComplete?.Invoke()); */
        }
    }
}