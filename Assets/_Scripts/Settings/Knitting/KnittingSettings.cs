using UnityEngine;

[CreateAssetMenu(fileName = "Knitting Settings", menuName = "Scriptable Objects/Settings/Knitting Settings")]
public class KnittingSettings : ScriptableObject
{
    public float KnittingDuration;
    public int knitJumpAmount;
    public float RollingDuration;
    public float ConnectionDuration;
    public float ConnectionLerpSpeed;
}
