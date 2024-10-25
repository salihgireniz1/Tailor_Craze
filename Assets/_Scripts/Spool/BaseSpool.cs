using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class BaseSpool : MonoBehaviour
{
    [SerializeField]
    protected Transform _yarnHolder;
    [SerializeField]
    protected Transform _body;

    public List<Yarn> _contents = new();
    protected bool _inProgress;
    public float rollDir = 1f;
    public void Start()
    {
        // Rotate when in progress.
        Observable
        .EveryUpdate()
        .Where(_ => _inProgress)
        .Subscribe(_ =>
        {
            _body.Rotate(0f, 1080f / YarnController.Instance._rollDuration * rollDir * Time.deltaTime, 0f);
            foreach (var item in _contents)
            {
                item.Spline.RebuildImmediate();
            }
        }).AddTo(this);
    }
    protected abstract void RemoveContent(int index);
    public Yarn GetTopYarn()
    {
        if (IsEmpty) return null;
        return _contents[_contents.Count - 1];
    }
    [Button]
    public virtual async UniTask UnrollTopYarn()
    {
        if (_inProgress) return;
        _inProgress = true;

        var unroll = YarnController.Instance.Rolling(_contents[_contents.Count - 1], RollType.UnRoll, this);
        await unroll;

        RemoveContent(_contents.Count - 1);
        _inProgress = false;
        transform.rotation = Quaternion.identity;
        Debug.Log("Unrolled.");
    }
    public bool IsEmpty => _contents == null || _contents.Count == 0;
}