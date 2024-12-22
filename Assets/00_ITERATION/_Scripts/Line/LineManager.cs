using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

public enum LineType { StraightSpoolPlane, StraightMannequin }
public class LineManager : MonoSingleton<LineManager>
{
    public BaseLine<Mannequin>[] MannequinLines => LevelManager.CurrentLevel.MannequinLines;

    public void RandomizeArray(ref Mannequin[] mannequins)
    {
        var rnd = new Random();
        mannequins = mannequins
         .Select(x => (x, rnd.Next()))
         .OrderBy(tuple => tuple.Item2)
         .Select(tuple => tuple.Item1)
         .ToArray();
    }
    public bool AllMannequinsCleared()
    {
        return LevelManager.CurrentLevel.AllMannequinsCleared();
    }

    #region Obsolete Methods
    // private BaseLine<T> CreateLine<T>(LineType type = default) where T : IQueueable<T>
    // {
    //     switch (type)
    //     {
    //         case LineType.StraightSpoolPlane:
    //             return _spoolLinePrefab as BaseLine<T>;
    //         default:
    //             return null;
    //     }
    // }

    // [Button]
    // public void InitSpoolTables(SpoolPlane[] planes, int spoolTableLineCount)
    // {
    //     float startX = -(spoolTableLineCount - 1) * _spacing / 2; // Start position to center the lines

    //     for (int i = 0; i < spoolTableLineCount; i++)
    //     {
    //         // Calculate x position for the current line
    //         float xPosition = startX + i * _spacing;

    //         // Create the line
    //         var spoolLine = CreateLine<SpoolPlane>(LineType.StraightSpoolPlane);
    //         var lineInstance = Instantiate(spoolLine);

    //         // Select planes for the current line
    //         SpoolPlane[] assignedPlanes = planes
    //             .Where((plane, index) => index % spoolTableLineCount == i)
    //             .ToArray();

    //         // Set the line's position
    //         lineInstance.transform.position = new Vector3(xPosition, 0, 0); // Adjust y and z as needed

    //         // Initialize the line with assigned planes
    //         lineInstance.Initialize(assignedPlanes);
    //     }
    // }

    #endregion
}


