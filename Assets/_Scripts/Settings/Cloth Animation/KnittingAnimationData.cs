using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Knitting Animation", fileName = "Knitting Animation Data")]
public class KnittingAnimationData : ScriptableObject
{
    public float _animationDuration = 0.15f;
    public float _clothScaleMultiplier = 1.5f;
    public float _zForwardOffset = -1;
    public Vector3 _clothSelectionRotate = new Vector3(40f, 0f, 0f);
    public Vector3 _clothDeselectRotate = new Vector3(0f, -30f, 0f);
}
