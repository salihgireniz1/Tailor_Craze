using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class ClothKnit : Knit
{
    public Material knitMaterial;
    public override void InitializeKnit(Color32 color)
    {
        if (!knitMaterial) knitMaterial = GetComponent<Renderer>().material;
        knitMaterial.color = color;
        transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }
    public override UniTask Activate(float activisionDuration)
    {
        SoundManager.Instance.PlaySFX(SFXType.Knitted);
        gameObject.SetActive(true);
        return transform.DOScale(Vector3.one * targetScale, activisionDuration).SetEase(Ease.Linear).ToUniTask();
    }
}
