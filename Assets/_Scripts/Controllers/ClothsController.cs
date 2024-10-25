using System.Collections.Generic;

public class ClothsController : MonoSingleton<ClothsController>
{
    public List<Cloth> activeCloths = new();

    public ClothPart GetClothWithData(YarnData data)
    {
        for (int i = 0; i < activeCloths.Count; i++)
        {
            ClothPart tmpPart = activeCloths[i].GetFillablePart(data);
            if (tmpPart != null)
            {
                return tmpPart;
            }
        }
        return null;
    }
}
