using System;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Level Info", fileName = "Levels", order = 0)]
public class LevelContainer : ScriptableObject
{
    [TableList(ShowIndexLabels = true)]
    public LevelData[] Levels;
}

[Serializable]
public struct LevelData
{
    [FoldoutGroup("Spools At This Level")]
    [ListDrawerSettings(ShowFoldout = true, ShowIndexLabels = true)]
    public SpoolInfo[] LevelSpools;

    [FoldoutGroup("Cloths At This Level")]
    [ListDrawerSettings(ShowFoldout = true, ShowIndexLabels = true)]
    public FactoryCloth[] LevelCloths;
}

[Serializable]
public struct SpoolInfo
{
    [HorizontalGroup("Spools At This Level", Width = 0.3f)]
    public bool isRandom;

    [HorizontalGroup("Spools At This Level")]
    [HideIf("isRandom"), ListDrawerSettings(ShowFoldout = true)]
    // [ValidateInput("ValidateYarnsCount", "You can only have a maximum of 3 yarns.", InfoMessageType.Error)]
    public SpoolYarn[] Yarns;
    private bool ValidateYarnsCount()
    {
        if (Yarns == null) return true;
        if (Yarns.Length > 4)
        {
            Array.Resize(ref Yarns, 3);
        }
        return Yarns.Length <= 3;
    }
}

[Serializable]
public struct SpoolYarn
{
    public bool isHidden;
    public YarnType type;
}
