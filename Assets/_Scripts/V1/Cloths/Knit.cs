using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public abstract class Knit : MonoBehaviour
{
    public abstract Material KnitMaterial { get; set; }
    // public float _activitionDuration = 0.75f;
    public float targetScale = 1f;
    public Ease _activitionEasing = Ease.OutBack;
    public abstract void InitializeKnit(Color32 color);

    public abstract UniTask Activate(float activisionDuration);
}
