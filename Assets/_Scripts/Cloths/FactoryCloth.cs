using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class FactoryCloth : MonoBehaviour
{
    public ClothData[] myClothParts;
    [Button]
    public void InitializeCloth()
    {
        foreach (var clothPart in myClothParts)
        {
            clothPart.part.InitializePart(clothPart.colorType);
            clothPart.part.MyCloth = this;
        }
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