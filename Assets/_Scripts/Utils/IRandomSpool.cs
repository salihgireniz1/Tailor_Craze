using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public interface IRandomSpool
{
    SpoolInfo GetRandomSpoolInfo();
}

public class GetRandomForExistingCloths : IRandomSpool
{
    private Dictionary<YarnType, int> yarnCounts;
    private SpoolInfo[] levelSpools;

    void Initialize()
    {
        yarnCounts = new Dictionary<YarnType, int>();
        this.levelSpools = SpoolController.Instance._levelSpools;
        // Collect all yarn types and store their count
        foreach (var item in ClothsController.Instance.activeCloths)
        {
            List<YarnType> clothYarns = item.GetPartTypes();
            foreach (var yarn in clothYarns)
            {
                if (yarnCounts.ContainsKey(yarn))
                {
                    yarnCounts[yarn]++;
                }
                else
                {
                    yarnCounts[yarn] = 1;
                }
            }
        }
        yarnCounts = yarnCounts
        .OrderByDescending(pair => pair.Value)
        .ToDictionary(pair => pair.Key, pair => pair.Value);
    }
    public bool IsUniform()
    {
        var values = yarnCounts.Values;
        if (values.Count == 0)
            return false;

        var first = values.First();
        return values.Skip(1).All(v => v.Equals(first));
    }
    // Find the YarnType with the highest count
    private YarnType GetMostNeededYarnType()
    {
        YarnType type;
        if (IsUniform()) type = yarnCounts.LastOrDefault().Key;
        else type = yarnCounts.FirstOrDefault().Key;

        if (yarnCounts.Count > 1)
            yarnCounts.Remove(type); // If there are at least 2 elements in the array, remove given one.

        return type;
    }
    SpoolInfo GenerateMixed()
    {
        SpoolInfo info = new SpoolInfo();
        info.isRandom = false;
        info.Yarns = new SpoolYarn[3];

        int rndmIndex = Random.Range(0, 3);
        info.Yarns[rndmIndex] = new SpoolYarn() { isHidden = false, type = GetMostNeededYarnType() };

        for (int i = 0; i < 3; i++)
        {
            if (i == rndmIndex) continue;
            info.Yarns[i] = new SpoolYarn() { isHidden = false, type = YarnController.Instance.GetRandomYarnType() };
        }

        return info;
    }

    SpoolInfo GetRandomAmongLevel()
    {
        YarnType mostNeededYarn = GetMostNeededYarnType();

        // Find the first SpoolInfo with the most needed YarnType
        foreach (var spool in levelSpools)
        {
            if (spool.Yarns != null && spool.Yarns.Any(y => y.type == mostNeededYarn))
            {
                return spool;
            }
        }

        // If no match is found, return a random SpoolInfo using Unity's Random.Range
        return levelSpools[Random.Range(0, levelSpools.Length)];
    }
    // Get SpoolInfo that matches the most needed YarnType, or return a random one if no match is found
    public SpoolInfo GetRandomSpoolInfo()
    {
        Initialize();
        return GenerateMixed();
    }
}

