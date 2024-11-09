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
    private Vector3 _defaultScale;
    private float _defaultZPos;
    CancellationTokenSource selectionAnimTokenSource;
    private void Start()
    {
        _defaultScale = transform.localScale;
        _defaultZPos = transform.position.z;
        DeselectRotate().Forget();
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
    public bool IsRotating;
    public UniTask SelectRotate()
    {
        selectionAnimTokenSource?.Cancel();
        selectionAnimTokenSource = new();
        IsRotating = true;
        var rotate = transform
                .DORotate(ClothsController.Instance.KnittingAnimData._clothSelectionRotate, ClothsController.Instance.KnittingAnimData._animationDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() => IsRotating = false)
                .ToUniTask(cancellationToken: selectionAnimTokenSource.Token);

        var bringForward = transform
                .DOMoveZ(_defaultZPos + ClothsController.Instance.KnittingAnimData._zForwardOffset, ClothsController.Instance.KnittingAnimData._animationDuration)
                .SetEase(Ease.InBack)
                .ToUniTask(cancellationToken: selectionAnimTokenSource.Token);

        var scale = transform
                .DOScale(_defaultScale * ClothsController.Instance.KnittingAnimData._clothScaleMultiplier, ClothsController.Instance.KnittingAnimData._animationDuration)
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
                .DOScale(_defaultScale, ClothsController.Instance.KnittingAnimData._animationDuration)
                .SetEase(Ease.InBack)
                .ToUniTask(cancellationToken: selectionAnimTokenSource.Token);

        var bringBackward = transform
                .DOMoveZ(_defaultZPos, ClothsController.Instance.KnittingAnimData._animationDuration)
                .SetEase(Ease.InBack)
                .ToUniTask(cancellationToken: selectionAnimTokenSource.Token);

        var rotate = transform
                .DORotate(ClothsController.Instance.KnittingAnimData._clothDeselectRotate, ClothsController.Instance.KnittingAnimData._animationDuration)
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
