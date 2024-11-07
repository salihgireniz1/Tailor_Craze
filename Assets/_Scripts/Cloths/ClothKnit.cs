using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;

public class ClothKnit : Knit
{
    public Material knitMaterial;
    public override void InitializeKnit(Color32 color)
    {
        knitMaterial = GetComponent<Renderer>().sharedMaterial;
        knitMaterial.color = color;
        transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }
    public override UniTask Activate(float activisionDuration)
    {
        gameObject.SetActive(true);

        transform.localScale = new Vector3(1, 0, 1);
        SoundManager.Instance.PlaySFX(SFXType.Knitted);
        // transform.DOPunchScale(Vector3.one * targetScale, activisionDuration, 2, 0).ToUniTask();
        transform
        .DOScaleY(targetScale * 1.1f, activisionDuration * 2.5f)
        .SetEase(Ease.OutBounce)
        .OnComplete(() =>
        {
            transform
                    .DOScale(Vector3.one * targetScale, activisionDuration * 2f)
                    .SetEase(Ease.InBack);
        })
        .ToUniTask();
        return UniTask.Delay(TimeSpan.FromSeconds(activisionDuration));
    }
}
