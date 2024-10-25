using Dreamteck.Splines;
using R3;
using UnityEngine;

public class Yarn : MonoBehaviour, IConnect
{
    [SerializeField] private Transform connectionPoint;
    private void Awake()
    {
        if (!Tube) Tube = GetComponent<TubeGenerator>();
        if (!Spline) Spline = GetComponent<SplineComputer>();
        Position = new ReactiveProperty<Vector3>(connectionPoint.position);
    }
    public void InitializeYarn(YarnData data)
    {
        if (!Tube) Tube = GetComponent<TubeGenerator>();
        if (!Spline) Spline = GetComponent<SplineComputer>();

        Data = data;
        Tube.color = data.color;
    }
    public TubeGenerator Tube { get; private set; }
    public SplineComputer Spline { get; private set; }
    [field: SerializeField] public YarnData Data { get; private set; }

    public ReactiveProperty<Vector3> Position { get; private set; }
}

public interface IConnect
{
    public ReactiveProperty<Vector3> Position { get; }
}