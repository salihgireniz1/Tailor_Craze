using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "Spool Animation Settings", menuName = "Scriptable Objects/Spool Animation Settings")]
public class SpoolAnimationSettings : ScriptableObject
{
    public float SpoolUnscaleDuration = 0.2f;
    public Ease SpoolUnscaleEase = Ease.InBack;
    public float DelayBetweenOldAndNewSpool = 0.5f;
    public float NewSpoolStartScale = 0.15f;
    public float SpoolScaleDuration = 1f;
    public Ease SpoolScaleEase = Ease.OutBack;
}