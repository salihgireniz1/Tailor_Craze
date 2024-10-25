using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class Cloth : MonoBehaviour
{
    public ClothData[] myClothParts;
    [Button]
    public void InitializeCloth()
    {
        foreach (var clothPart in myClothParts)
        {
            clothPart.part.InitializePart(clothPart.colorType);
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
}
[Serializable]
public struct ClothData
{
    public ClothPart part;
    public YarnType colorType;
}