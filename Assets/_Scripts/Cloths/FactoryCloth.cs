using System;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;

public class FactoryCloth : MonoBehaviour
{
    public ClothData[] myClothParts;
    public float _clothScaleMultiplier = 1.5f;
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
        }
    }
    public UniTask SelectRotate()
    {
        selectionAnimTokenSource?.Cancel();
        selectionAnimTokenSource = new();
        var rotate = transform
                .DORotate(ClothsController.Instance.bandAnimData._clothSelectionRotate, ClothsController.Instance.bandAnimData._animationDuration)
                .SetEase(Ease.InBack)
                .ToUniTask(cancellationToken: selectionAnimTokenSource.Token);

        var bringForward = transform
                .DOMoveZ(_defaultZPos + ClothsController.Instance.bandAnimData._zForwardOffset, ClothsController.Instance.bandAnimData._animationDuration)
                .SetEase(Ease.InBack)
                .ToUniTask(cancellationToken: selectionAnimTokenSource.Token);

        var scale = transform
                .DOScale(_defaultScale * _clothScaleMultiplier, ClothsController.Instance.bandAnimData._animationDuration)
                .SetEase(Ease.InBack)
                .ToUniTask(cancellationToken: selectionAnimTokenSource.Token);

        return UniTask.WhenAll(rotate, scale, bringForward);
    }
    public UniTask DeselectRotate()
    {
        selectionAnimTokenSource?.Cancel();
        selectionAnimTokenSource = new();
        var scale = transform
                .DOScale(_defaultScale, ClothsController.Instance.bandAnimData._animationDuration)
                .SetEase(Ease.InBack)
                .ToUniTask(cancellationToken: selectionAnimTokenSource.Token);

        var bringBackward = transform
                .DOMoveZ(_defaultZPos, ClothsController.Instance.bandAnimData._animationDuration)
                .SetEase(Ease.InBack)
                .ToUniTask(cancellationToken: selectionAnimTokenSource.Token);

        var rotate = transform
                .DORotate(ClothsController.Instance.bandAnimData._clothDeselectRotate, ClothsController.Instance.bandAnimData._animationDuration)
                .SetEase(Ease.InBack)
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
}
