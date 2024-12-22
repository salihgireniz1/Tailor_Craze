using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dreamteck.Splines.Primitives;
using Dreamteck.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

public class Level : MonoBehaviour
{
    public int LevelIndex;
    public LevelInfo LevelInfo;
    public GameData Data;

    [Title("Current Lines"), Space]
    public SpoolLine[] SpoolLines;
    public BaseLine<Mannequin>[] MannequinLines;

    public List<MannequinLineInfo> MannequinLineInfos = new();
    public List<SpoolLineInfo> SpoolLineInfos = new();

    [Button]
    public void Initialize()
    {
        FindLines();

        foreach (var item in SpoolLines)
        {
            item.ClearLine();
            item.LineContent = new();

            SpoolLineInfos.Add(new SpoolLineInfo() { Line = item, Content = new() });
        }
        foreach (var item in MannequinLines)
        {
            item.ClearLine();
            item.Content = new();

            MannequinLineInfos.Add(new MannequinLineInfo() { Line = item, Content = new() });
        }
        int manIndex = 0;

        // Iterate through all determined plane data(s) at this level.
        for (int i = 0; i < LevelInfo.PlaneData.Length; i++)
        {
            PlaneSpools planeSpools = LevelInfo.PlaneData[i].PlaneSpools;

            // Distribute level plane infos to each spool line one by one.
            // Therefore, regardless of the line count, we will have well designed spool lines.
            // SpoolLines[i % SpoolLines.Length].LineContent.Add(planeSpools);


            foreach (var line in SpoolLineInfos)
            {
                if (line.Line == SpoolLines[i % SpoolLines.Length])
                {
                    line.Content.Add(planeSpools);
                }
            }

            for (int j = 0; j < planeSpools.SpoolTypes.Count; j++)
            {
                YarnType currentType = planeSpools.SpoolTypes[j];

                var newMannequin = Data.GetMannequinPrefab(currentType);
                BaseLine<Mannequin> baseLine = MannequinLines[manIndex % MannequinLines.Length];

                foreach (var line in MannequinLineInfos)
                {
                    if (line.Line == baseLine)
                    {
                        line.Content.Add(newMannequin);
                    }
                }

                manIndex++;
            }
        }
        foreach (var line in MannequinLineInfos)
        {
            line.Content.Shuffle();
        }
    }

    public bool AllMannequinsCleared()
    {
        return MannequinLines.All(line => line.PeekFirst() == default);
    }

    private void FindLines()
    {
        SpoolLines = GetComponentsInChildren<SpoolLine>();
        MannequinLines = GetComponentsInChildren<BaseLine<Mannequin>>();

        MannequinLineInfos = new();
        SpoolLineInfos = new();
    }
}
[Serializable]
public struct MannequinLineInfo
{
    public List<Mannequin> Content;
    public BaseLine<Mannequin> Line;
}

[Serializable]
public struct SpoolLineInfo
{
    public List<PlaneSpools> Content;
    public SpoolLine Line;
}