using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;

public class ClothKnit : Knit
{
    public Material knitMaterial;
    public override Material KnitMaterial { get => knitMaterial; set => knitMaterial = value; }
    public override void InitializeKnit(Color32 color)
    {
        GetComponent<Renderer>().sharedMaterial = knitMaterial;
        knitMaterial.color = color;
        transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }
    public async override UniTask Activate(float activisionDuration)
    {
        // double t = (double)Time.time;
        gameObject.SetActive(true);

        // transform.localScale = new Vector3(1, 1, 0);
        // transform.DOPunchScale(Vector3.one * targetScale, activisionDuration, 2, 0).ToUniTask();
        transform
        .DOScale(targetScale * 2.5f, activisionDuration * 1.5f)
        .SetEase(Ease.OutBounce)
        .OnComplete(() =>
        {
            transform
                    .DOScale(Vector3.one * targetScale, activisionDuration * 1f)
                    .SetEase(Ease.InBack);
        });

        await UniTask.Delay(TimeSpan.FromSeconds((double)activisionDuration));
        // Debug.Log($" Expected: {activisionDuration} --- Reality: {(double)Time.time - t}");
        // return UniTask.DelayFrame(1);
    }
}
