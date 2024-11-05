using System;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;

public class FactoryCloth : MonoBehaviour
{
    public ClothData[] myClothParts;
    CancellationTokenSource rotationTokenSource;
    private void Start()
    {
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
        rotationTokenSource?.Cancel();
        rotationTokenSource = new();
        return transform.DORotate(new Vector3(40f, 0f, 0f), .2f).SetEase(Ease.InBack).ToUniTask(cancellationToken: rotationTokenSource.Token);
    }
    public UniTask DeselectRotate()
    {
        rotationTokenSource?.Cancel();
        rotationTokenSource = new();
        return transform.DORotate(new Vector3(0f, -30f, 0f), .2f).SetEase(Ease.InBack).ToUniTask(cancellationToken: rotationTokenSource.Token);
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