using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;

public class ImageKnit : Knit
{
    public SpriteRenderer image;
    private void Awake()
    {
        image = GetComponent<SpriteRenderer>();
    }
    public override void InitializeKnit(Color32 color)
    {
        if (!image) image = GetComponent<SpriteRenderer>();
        image.color = color;
        transform.localScale = Vector3.zero;
    }
    public override UniTask Activate(float activisionDuration)
    {
        SoundManager.Instance.PlaySFX(SFXType.Knitted);
        // transform.DOPunchScale(Vector3.one * targetScale, activisionDuration, 2, 0).ToUniTask();
        transform
        .DOScale(Vector3.one * targetScale * 2f, activisionDuration)
        .SetEase(Ease.OutBounce)
        .OnComplete(() =>
        {
            transform
                    .DOScale(Vector3.one * targetScale, activisionDuration)
                    .SetEase(Ease.InBack);
        })
        .ToUniTask();
        return UniTask.Delay(TimeSpan.FromSeconds(activisionDuration));
    }
}