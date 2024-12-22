using UnityEngine;

public class DeskSpot : MonoBehaviour
{
    public Vector3 Position => _transform.position;
    public SpoolPlane ActivePlane { get; set; }
    public bool IsOccupied => ActivePlane != null;
    private Transform _transform;
    private void Awake()
    {
        _transform = transform;
    }
}
