using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Band Movement Animation", fileName = "Band Movement Animation Data")]
public class BandAnimationData : ScriptableObject
{
    public Vector3 spotOffset;
    public float shiftingDuration;
    public Ease shiftingEase;
}