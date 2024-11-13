using UnityEngine;
using System;
[CreateAssetMenu(menuName = "Scriptable Objects/Yarn Data", fileName = "Yarn Data Container")]
public class YarnDataContainer : ScriptableObject
{
    public Yarn BaseYarn;
    public HiddenYarn HiddenYarn;
    public YarnData[] yarnGroups;
}
public enum YarnType { K, P, M, S, T, O, Y }