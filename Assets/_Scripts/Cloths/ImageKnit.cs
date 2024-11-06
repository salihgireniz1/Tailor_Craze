using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

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
        return transform.DOScale(Vector3.one * targetScale, activisionDuration).SetEase(Ease.Linear).ToUniTask();
    }
}