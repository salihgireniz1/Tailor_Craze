using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IQueueable<T> where T : IQueueable<T>
{
    public BaseLine<T> CurrentLine { get; set; }
    public Transform CurrentStandPoint { get; set; }
    public UniTask Move(Vector3 position, float duration = 0F);
    public UniTask AlignToPoint(Transform newPoint, float duration = 0F);
}
