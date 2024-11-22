using System;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

public class FactoryCloth : MonoBehaviour
{
    public ClothData[] myClothParts;
    public bool IsRotating;
    private Vector3 _defaultScale;
    private float _defaultZPos;
    [SerializeField] private Animator _mannequinAnimator;
    CancellationTokenSource selectionAnimTokenSource;
    private void Start()
    {
        _mannequinAnimator = GetComponentInChildren<Animator>();
        _defaultScale = transform.localScale;
        _defaultZPos = transform.position.z;
        transform.DORotate(Settings.KnittingAnimationData._clothDeselectRotate, 0f);
        // DeselectRotate().Forget();
    }
    public void AdjustmentShakeAnim(bool isFirst = false)
    {
        if (_mannequinAnimator != null && !isFirst)
        {
            _mannequinAnimator.ResetTrigger("Shake");
            _mannequinAnimator.speed = UnityEngine.Random.Range(.9f, 1.1f);
            _mannequinAnimator.SetTrigger("Shake");
        }
    }
    [Button]
    public void InitializeCloth()
    {
        foreach (var clothPart in myClothParts)
        {
            clothPart.part.InitializePart(clothPart.colorType);
            clothPart.part.MyCloth = this;
            clothPart.part._requiredYarnCount = clothPart.RequiredYarnCount;
        }
    }
    public UniTask SelectRotate()
    {
        selectionAnimTokenSource?.Cancel();
        selectionAnimTokenSource = new();
        IsRotating = true;
        var rotate = transform
                .DORotate(Settings.KnittingAnimationData._clothSelectionRotate, Settings.KnittingAnimationData._animationDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() => IsRotating = false)
                .ToUniTask(cancellationToken: selectionAnimTokenSource.Token);

        var bringForward = transform
                .DOMoveZ(_defaultZPos + Settings.KnittingAnimationData._zForwardOffset, Settings.KnittingAnimationData._animationDuration)
                .SetEase(Ease.InBack)
                .ToUniTask(cancellationToken: selectionAnimTokenSource.Token);

        var scale = transform
                .DOScale(_defaultScale * Settings.KnittingAnimationData._clothScaleMultiplier, Settings.KnittingAnimationData._animationDuration)
                .SetEase(Ease.InBack)
                .ToUniTask(cancellationToken: selectionAnimTokenSource.Token);

        return UniTask.WhenAll(rotate, scale, bringForward);
    }
    public UniTask DeselectRotate()
    {
        selectionAnimTokenSource?.Cancel();
        selectionAnimTokenSource = new();
        IsRotating = true;
        var scale = transform
                .DOScale(_defaultScale, Settings.KnittingAnimationData._animationDuration)
                .SetEase(Ease.InBack)
                .ToUniTask(cancellationToken: selectionAnimTokenSource.Token);

        var bringBackward = transform
                .DOMoveZ(_defaultZPos, Settings.KnittingAnimationData._animationDuration)
                .SetEase(Ease.InBack)
                .ToUniTask(cancellationToken: selectionAnimTokenSource.Token);

        var rotate = transform
                .DORotate(Settings.KnittingAnimationData._clothDeselectRotate, Settings.KnittingAnimationData._animationDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() => IsRotating = false)
                .ToUniTask(cancellationToken: selectionAnimTokenSource.Token);

        return UniTask.WhenAll(rotate, scale, bringBackward);
    }
    public ClothPart GetFillablePart(YarnData data)
    {
        for (int i = 0; i < myClothParts.Length; i++)
        {
            if (myClothParts[i].part.CanBeFilled(data))
            {
                return myClothParts[i].part;
            }
        }
        return null;
    }
    public List<YarnType> GetPartTypes()
    {
        List<YarnType> types = new List<YarnType>();
        foreach (var item in myClothParts)
        {
            if (!item.part.IsFilled)
            {
                types.Add(item.part.Type);
            }
        }
        return types;
    }
    public bool IsCompleted()
    {
        foreach (var item in myClothParts)
        {
            if (!item.part.IsFilled)
            {
                return false;
            }
        }
        return true;
    }
}
[Serializable]
public struct ClothData
{
    public ClothPart part;
    public YarnType colorType;
    public int RequiredYarnCount;
}
