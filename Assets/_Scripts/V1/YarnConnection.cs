using Cysharp.Threading.Tasks;
using DG.Tweening;
using Dreamteck.Splines;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

public class YarnConnection : MonoSingleton<YarnConnection>
{
    public IConnect StartConnect { get; private set; }
    public IConnect EndConnect { get; private set; }
    public TubeGenerator tube;
    private Node startNode;
    private Node endNode;
    bool isActive;
    DisposableBag bag;

    [Title("Rope Shake Settings")]
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeStrength = 100f;
    [SerializeField] private int shakeVibrato = 10;
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
        if (isActive)
        {
            endNode.transform.DOMove(EndConnect.Position, 0.1f).SetEase(Ease.Linear);
            startNode.transform.DOMove(StartConnect.Position, 0.1f).SetEase(Ease.Linear);
        }
        else
        {
            endNode.transform.position = EndConnect.Position;
            startNode.transform.position = StartConnect.Position;
        }
        endNode.transform.DOShakeRotation(shakeDuration, shakeStrength, shakeVibrato, 90, true, ShakeRandomnessMode.Harmonic);
        startNode.transform.DOShakeRotation(shakeDuration, shakeStrength, shakeVibrato, 90, true, ShakeRandomnessMode.Harmonic);
    }
    public async UniTask ActivateConnection(YarnData data)
    {
        if (isActive) return;

        // tube.color = data.color;
        // myMat.color = data.color;
        GetComponent<Renderer>().sharedMaterial = data.yarnMaterial;
        isActive = true;
        Observable.EveryUpdate()
            .Where(_ => isActive && tube != null)
            .Subscribe(_ =>
            {
                tube.uvOffset += Vector2.right * Time.deltaTime;
                if (EndConnect != null)
                {
                    endNode.transform.position = Vector3.Lerp(
                                        endNode.transform.position,
                                        EndConnect.Position,
                                        Time.deltaTime * Settings.KnittingSettings.ConnectionLerpSpeed);
                }
            })
            .AddTo(ref bag);

        await DOTween.To(
            () => tube.clipTo,
            clip => tube.clipTo = clip,
            1f,
            Settings.KnittingSettings.ConnectionDuration)
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
            Settings.KnittingSettings.ConnectionDuration)
            .SetEase(Ease.Linear)
            .ToUniTask();
        isActive = false;
        tube.clipFrom = 0;
        tube.clipTo = 0;
        bag.Clear();
    }
}
