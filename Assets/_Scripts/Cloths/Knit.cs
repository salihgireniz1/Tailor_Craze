using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

public class Knit : MonoBehaviour
{
    // public float _activitionDuration = 0.75f;
    public float targetScale = 0.15f;
    public Ease _activitionEasing = Ease.OutBack;
    public SpriteRenderer image;
    private void Awake()
    {
        image = GetComponent<SpriteRenderer>();
    }
    public void InitializeKnit(Color32 color)
    {
        if (!image) image = GetComponent<SpriteRenderer>();
        image.color = color;
        transform.localScale = Vector3.zero;
    }
    public UniTask Activate(float activisionDuration)
    {
        SoundManager.Instance.PlaySFX(SFXType.Knitted);
        return transform.DOScale(Vector3.one * targetScale, activisionDuration).SetEase(Ease.Linear).ToUniTask();
    }
}