using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using UnityEngine;

public class DistributionManager : MonoSingleton<DistributionManager>
{
    [SerializeField] private ParticleSystem _spoolParticle;
    Mannequin[] _frontMannequins;

    IFillable latestFillable;

    UniTask fill;

    ClothPart clothPart;
    private bool _inProgress;
    public SerializableReactiveProperty<int> CompletedMan { get; private set; } = new();
    public SerializableReactiveProperty<float> ProgressPercent { get; private set; } = new();
    private void Start()
    {
        GameManager.CurrentState.Subscribe(state =>
        {
            if (state == GameState.Initializing)
            {
                CompletedMan.Value = 0;
            }
        }).AddTo(this);
        CompletedMan.Subscribe(completedMan =>
        {
            int total = LevelManager.CurrentLevel?.LevelInfo.PlaneData.Length * 3 ?? 0;
            if (total > 0)
            {
                ProgressPercent.Value = (float)completedMan / total;
            }
            else
            {
                ProgressPercent.Value = 0; // Default to 0 progress if totalMan is 0 or invalid
            }
        }).AddTo(this);
    }
    public async UniTask Distribute(SpoolPlane plane)
    {
        while (_inProgress)
        {
            await UniTask.Yield();
        }
        _inProgress = true;
        foreach (var spool in plane.Spools)
        {
            if (spool != null)
            {
                await KnitMannequin(spool);
            }
        }


        if (plane && plane.IsEmpty)
        {
            // Instantiate(_spoolParticle, plane.transform.position, Quaternion.identity);

            await plane.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).ToUniTask();
            SoundManager.Instance.PlaySFX(SFXType.TrayDisappear);

            _spoolParticle?.Stop();
            _spoolParticle.transform.position = plane.transform.position;
            _spoolParticle?.Play();
            Destroy(plane.gameObject);
        }

        _inProgress = false;
    }

    public async UniTask KnitMannequin(Spool spool)
    {
        Yarn topYarn = spool.GetTopYarn();

        // Spool cleared. Break process.
        if (topYarn == null)
        {
            await YarnConnection.Instance.BreakConnection();
            await fill;
            fill = UniTask.CompletedTask;

            spool.RemainBase.SetParent(spool.transform.parent);

            spool.transform
                .DOScale(Vector3.zero, Settings.SpoolAnimationSettings.SpoolUnscaleDuration)
                .SetEase(Settings.SpoolAnimationSettings.SpoolUnscaleEase)
                .ToUniTask().Forget();

            DestroyImmediate(spool.gameObject);

            // Dequeue first mannequin and complete it.
            foreach (var mannequin in _frontMannequins)
            {
                if (mannequin != default && mannequin.FactoryCloth.IsCompleted())
                {
                    CompleteMannequin(mannequin).Forget();
                }
            }

            return;
        }

        // Need to check if we keep knitting same cloth.
        // To do this:

        // Find current cloth match.
        clothPart = CheckMatch(topYarn.Data);

        // This is the condition that checks if there is a cloth match and it is the same cloth with the previous cloth.
        bool continueKnitting = clothPart && latestFillable == (IFillable)clothPart;

        // If we don't continue with same color, break the connection and wait previous cloth part to complete.
        if (!continueKnitting)
        {
            await YarnConnection.Instance.BreakConnection();
            await fill;
        }
        /* 
                // If we completed every possible cloth or somehow failed, break the process.
                if (GameManager.CurrentState.Value == GameState.Victory || GameManager.CurrentState.Value == GameState.GameOver)
                {
                    await YarnConnection.Instance.BreakConnection();
                    return;
                }

                // If we got here, it means we have some yarns to knit clothes.
                // Set the current state to in progress.
                GameManager.CurrentState.Value = GameState.InProgress;
         */

        // Avoid recalculations and find match by checking if we found cloth above,
        // if not, get an empty deposit.
        IFillable match = clothPart;

        // Empty clothpart to avoid false comparisons recursively.
        clothPart = null;

        // Since this match will be filled surely, we can assign it as
        // the latest fillable for later comparisons.
        if (match != latestFillable)
        {
            latestFillable = match;
            await YarnConnection.Instance.BreakConnection();
        }

        if (match == null) return;

        await spool.BringForward();

        // Connect yarn to the match.
        YarnConnection.Instance.SetConnectionPoints(topYarn, match.Connector);
        YarnConnection.Instance.ActivateConnection(topYarn.Data).Forget();

        // Calculate how long it will take to fill the current match.
        var duration = match.FillDuration;
        // Start filling the current match.
        fill = match.Fill(topYarn.Data);

        // Unroll the yarn from the spool for fill time duration. Wait until its done.
        var unroll = spool.UnrollTopYarn(duration);
        await unroll;

        // After unrolling, continue with next yarn from the spool.
        await KnitMannequin(spool);
    }

    public ClothPart CheckMatch(YarnData spoolData)
    {
        _frontMannequins = new Mannequin[LineManager.Instance.MannequinLines.Length];
        for (int i = 0; i < LineManager.Instance.MannequinLines.Length; i++)
        {
            var lineFirstMannequin = LineManager.Instance.MannequinLines[i].PeekFirst();
            _frontMannequins[i] = lineFirstMannequin;
        }
        foreach (var mannequin in _frontMannequins)
        {
            if (mannequin == default) continue;

            var clothPart = mannequin.FactoryCloth.GetFillablePart(spoolData);
            if (clothPart)
            {
                return clothPart;
            }
        }

        return default;
    }

    public async UniTask CompleteMannequin(Mannequin mannequin)
    {

        await mannequin.transform
                    .DOMoveY(mannequin.transform.position.y + 2f, .15f)
                    .SetEase(Ease.Linear)
                    .ToUniTask();

        mannequin.CurrentLine.ReturnFirst();
        mannequin.CurrentLine.OrderQueue().Forget();
        CompletedMan.Value++;
        DeskManager.Instance.CheckSpotPlanes().Forget();

        await mannequin.transform
            .DOMoveX(mannequin.transform.position.x + 15f, .4f)
            .SetEase(Ease.InBack)
            .ToUniTask();


        DeskManager.Instance.CheckSpotPlanes().Forget();
        Destroy(mannequin.gameObject);
    }
}