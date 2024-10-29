using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Yarn Data", fileName = "Yarn Data Container")]
public class YarnDataContainer : ScriptableObject
{
    public Yarn BaseYarn;
    public HiddenYarn HiddenYarn;
    public YarnData[] yarnGroups;
}
public enum YarnType { red, green, blue, yellow, orange }