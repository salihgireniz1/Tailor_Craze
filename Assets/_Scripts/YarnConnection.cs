using Cysharp.Threading.Tasks;
using DG.Tweening;
using Dreamteck.Splines;
using R3;
using UnityEngine;

public class YarnConnection : MonoSingleton<YarnConnection>
{
    public IConnect StartConnect { get; private set; }
    public IConnect EndConnect { get; private set; }
    public TubeGenerator tube;
    public float connectDuration = 1f;
    private Node startNode;
    private Node endNode;
    bool isActive;
    DisposableBag bag;
    public float lerpSpeed = 20f;
    protected override void Awake()
    {
        base.Awake();
        tube = GetComponent<TubeGenerator>();
        GetNodes();
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        bag.Dispose();
    }
    void GetNodes()
    {
        var nodes = tube.spline.GetNodes();
        startNode = nodes[0];
        endNode = nodes[nodes.Count - 1];
    }
    public void SetConnectionPoints(IConnect from, IConnect to)
    {
        this.EndConnect = to;
        this.StartConnect = from;
        // endNode.transform.DOMove(EndConnect.Position.Value, 0.1f).SetEase(Ease.Linear);
        // startNode.transform.DOMove(StartConnect.Position.Value, 0.1f).SetEase(Ease.Linear);

        endNode.transform.position = EndConnect.Position.Value;
        startNode.transform.position = StartConnect.Position.Value;

        // EndConnect.Position.Subscribe(pos =>
        // {
        //     endNode.transform.position = pos;
        //     Debug.Log(pos);
        // })
        // .AddTo(ref bag);
        // StartConnect.Position.Subscribe(pos =>
        // {
        //     startNode.transform.position = pos;
        //     // Debug.Log(pos);
        // })
        // .AddTo(ref bag);
    }
    public async UniTask ActivateConnection(YarnData data)
    {
        if (isActive) return;

        tube.color = data.color;
        isActive = true;
        Observable.EveryUpdate()
            .Where(_ => isActive && tube != null)
            .Subscribe(_ =>
            {
                tube.uvOffset += Vector2.right * Time.deltaTime;
                endNode.transform.position = Vector3.Lerp(
                    endNode.transform.position,
                    EndConnect.Position.Value,
                    Time.deltaTime * lerpSpeed);
            })
            .AddTo(ref bag);

        await DOTween.To(
            () => tube.clipTo,
            clip => tube.clipTo = clip,
            1f,
            connectDuration)
            .SetEase(Ease.Linear)
            .ToUniTask();


    }
    public async UniTask BreakConnection()
    {
        if (!isActive) return;
        await DOTween.To(
            () => tube.clipFrom,
            clip => tube.clipFrom = clip,
            1f,
            connectDuration)
            .SetEase(Ease.Linear)
            .ToUniTask();
        isActive = false;
        tube.clipFrom = 0;
        tube.clipTo = 0;
        bag.Clear();
    }
}
