using Dreamteck.Splines;
using UnityEngine;

public class Yarn : MonoBehaviour, IConnect
{
    [SerializeField] private Transform connectionPoint;
    private void Awake()
    {
        if (!Tube) Tube = GetComponent<TubeGenerator>();
        if (!Spline) Spline = GetComponent<SplineComputer>();
    }
    public virtual void InitializeYarn(YarnData data)
    {
        if (!Tube) Tube = GetComponent<TubeGenerator>();
        if (!Spline) Spline = GetComponent<SplineComputer>();

        Data = data;
        // Tube.color = data.color;
        var r = GetComponent<Renderer>();
        r.material = data.yarnMaterial;
        // r.material.color = data.color;

        Spline.RebuildImmediate();
    }
    [field: SerializeField] public YarnData Data { get; protected set; }
    public TubeGenerator Tube { get; protected set; }
    public SplineComputer Spline { get; protected set; }
    public Vector3 Position { get => connectionPoint.position; }
}
