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
    [FoldoutGroup("Spools At This Level")]
    public bool RandomNextSpool;

    [FoldoutGroup("Cloths At This Level")]
    [ListDrawerSettings(ShowFoldout = true, ShowIndexLabels = true)]
    public FactoryCloth[] LevelCloths;
}
[Serializable]
public struct ClothInfo
{
    [HorizontalGroup("Cloths At This Level", Width = 0.3f)]
    public int RequiredYarnCount;

    [HorizontalGroup("Cloths At This Level")]
    public FactoryCloth ClothPrefab;

    [HorizontalGroup("Cloths At This Level")]
    public YarnType colorType;
}
[Serializable]
public struct SpoolInfo
{
    [HorizontalGroup("Spools At This Level", Width = 0.3f)]
    public bool isRandom;

    [HorizontalGroup("Spools At This Level")]
    [HideIf("isRandom"), ListDrawerSettings(ShowFoldout = true)]
    public SpoolYarn[] Yarns;
}

[Serializable]
public struct SpoolYarn
{
    public bool isHidden;
    public YarnType type;
}
