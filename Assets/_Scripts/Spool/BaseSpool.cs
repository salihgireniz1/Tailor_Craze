using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
    private float _rollDuration = 1f;
    public void Build()
    {
        foreach (var item in _contents)
        {
            item.Spline.RebuildImmediate();
        }
    }
    public void Start()
    {
        // Rotate when in progress.
        Observable
        .EveryUpdate()
        .Where(_ => _inProgress)
        .Subscribe(_ =>
        {
            _body.Rotate(0f, 1080f / _rollDuration * rollDir * Time.deltaTime, 0f);
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
    public virtual async UniTask UnrollTopYarn(float duration)
    {
        if (_inProgress) return;
        _rollDuration = duration;
        _inProgress = true;
        var unroll = YarnController.Instance.Rolling(GetTopYarn(), RollType.UnRoll, this, duration);
        await unroll;

        RemoveContent(_contents.Count - 1);
        _inProgress = false;
        transform.rotation = Quaternion.identity;
    }
    public bool IsEmpty => _contents == null || _contents.Count == 0;
}