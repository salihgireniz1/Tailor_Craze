using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public interface IRandomSpool
{
    SpoolInfo GetRandomSpoolInfo();
}

public class GetRandomForExistingCloths : IRandomSpool
{
    private Dictionary<YarnType, int> yarnCounts = new Dictionary<YarnType, int>();
    private SpoolInfo[] levelSpools;

    void Initialize()
    {

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
    }

    // Find the YarnType with the highest count
    private YarnType GetMostNeededYarnType()
    {
        return yarnCounts.OrderByDescending(y => y.Value).FirstOrDefault().Key;
    }

    // Get SpoolInfo that matches the most needed YarnType, or return a random one if no match is found
    public SpoolInfo GetRandomSpoolInfo()
    {
        Initialize();
        YarnType mostNeededYarn = GetMostNeededYarnType();

        // Find the first SpoolInfo with the most needed YarnType
        foreach (var spool in levelSpools)
        {
            if (spool.Yarns != null && spool.Yarns.Any(y => y.type == mostNeededYarn))
            {
                // Debug.Log(spool.Yarns[0].type + "/" + spool.Yarns[1].type);
                return spool;
            }
        }

        // If no match is found, return a random SpoolInfo using Unity's Random.Range
        return levelSpools[Random.Range(0, levelSpools.Length)];
    }
}

