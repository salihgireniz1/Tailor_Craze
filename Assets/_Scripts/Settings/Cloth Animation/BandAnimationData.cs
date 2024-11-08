using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Band Movement Animation", fileName = "Band Movement Animation Data")]
public class BandAnimationData : ScriptableObject
{
    public float shiftingDuration;
    public Ease shiftingEase;
}