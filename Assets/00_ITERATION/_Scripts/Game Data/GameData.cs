using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Data", menuName = "Scriptable Objects/GameData", order = 0)]
public class GameData : ScriptableObject
{
    [Title("Mannequin Configuration")]
    [Tooltip("Prefabs for mannequins used in the game")]
    public Mannequin[] MannequinPrefabs;

    [Title("Plane Configuration")]
    [Tooltip("Prefab for the spool plane")]
    public SpoolPlane PlanePrefab;

    public Mannequin GetMannequinPrefab(YarnType type)
    {
        return MannequinPrefabs.FirstOrDefault(prefab => prefab.FactoryCloth.Type == type);
    }
}

[Serializable]
public struct LevelInfo
{
    public PlaneData[] PlaneData;
}

[Serializable]
public struct PlaneSpools
{
    public List<YarnType> SpoolTypes;

}

[Serializable]
public struct PlaneData
{
    public TableType type;

    public PlaneSpools PlaneSpools;

    private List<YarnType> GetPossibleColors(int count)
    {
        // Get all possible values from the YarnType enum
        YarnType[] allYarns = (YarnType[])Enum.GetValues(typeof(YarnType));

        // Ensure the colorCount does not exceed the available yarn types
        count = Math.Min(count, allYarns.Length);

        // Create and populate the result array with the first `colorCount` YarnType values
        YarnType[] result = new YarnType[count];
        Array.Copy(allYarns, result, count);
        return result.ToList();
    }
    [Button]
    public void FillContent(int count = 3)
    {
        List<YarnType> possibleColors = GetPossibleColors(count);
        List<YarnType> selectedColors = new List<YarnType>();

        for (int i = 0; i < (int)type; i++)
        {
            int rndIndex = UnityEngine.Random.Range(0, possibleColors.Count);
            selectedColors.Add(possibleColors[rndIndex]);
            possibleColors.RemoveAt(rndIndex);
        }
        PlaneSpools.SpoolTypes = new();
        for (int i = 0; i < 3; i++)
        {
            YarnType color = selectedColors[i % selectedColors.Count];
            PlaneSpools.SpoolTypes.Add(color);
        }
    }
}
public enum TableType
{
    three = 1,
    twoone = 2,
    oneoneone = 3
}