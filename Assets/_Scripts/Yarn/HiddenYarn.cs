using Cysharp.Threading.Tasks;
using Dreamteck.Splines;
using UnityEngine;

public class HiddenYarn : Yarn, IReveal
{
    [SerializeField] private YarnType _revealedType;
    public YarnType RevealedType { get => _revealedType; set => _revealedType = value; }

    public UniTask Reveal()
    {
        Tube.color = Data.color;
        var r = GetComponent<Renderer>();
        r.material = Data.material;
        r.material.color = Data.color;

        Spline.RebuildImmediate();
        return UniTask.CompletedTask;
    }
    public override void InitializeYarn(YarnData data)
    {
        if (!Tube) Tube = GetComponent<TubeGenerator>();
        if (!Spline) Spline = GetComponent<SplineComputer>();
        Data = data;

    }
}
public interface IReveal
{
    public YarnType RevealedType { get; }
    public UniTask Reveal();
}