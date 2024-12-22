using System.Collections.Generic;

using UnityEngine;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

public class StraightLine<T> : BaseLine<T> where T : IQueueable<T>
{
    [Title("Initialize Settings")]
    [SerializeField] protected float _lineOffset;
    [SerializeField] protected int _lineStartZ;

    [Button(parameterBtnStyle: ButtonStyle.FoldoutButton)]
    public override async UniTask Initialize(T[] values)
    {
        ClearLine();

        LineQueue = new();
        LinePoints = new Transform[values.Length];

        var zPos = new Vector3(transform.position.x, transform.position.y, _lineStartZ);
        transform.position = zPos;

        for (int i = 0; i < values.Length; i++)
        {
            GameObject linePointObject = new GameObject($"LinePoint_{i}");
            linePointObject.transform.SetParent(this.transform);
            linePointObject.transform.localPosition = new Vector3(0, 0, i * _lineOffset);
            LinePoints[i] = linePointObject.transform;
            AddToLine(values[i]);
            await values[i].AlignToPoint(linePointObject.transform, 0F);
        }
    }
    public override Queue<T> LineQueue { get; set; }
    public override Transform[] LinePoints { get; set; }
}