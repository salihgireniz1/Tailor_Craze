using System.Linq;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

public class SelectionController : MonoSingleton<SelectionController>
{
    public ReactiveProperty<Spool> SelectedSpool = new ReactiveProperty<Spool>();

    private void Start()
    {
        SelectedSpool
        .Where(spool => spool != null && GameManager.CurrentState.Value == GameState.Playing)
        .Subscribe(
            async spool =>
            {
                await EmptySpool(spool);
            }
        )
        .AddTo(this);
    }
    IFillable latestFillable;
    public async UniTask EmptySpool(BaseSpool spool)
    {
        if (GameManager.CurrentState.Value == GameState.Victory || GameManager.CurrentState.Value == GameState.GameOver)
        {
            await YarnConnection.Instance.BreakConnection();
            return;
        }

        GameManager.CurrentState.Value = GameState.InProgress;
        if (spool.IsEmpty)
        {
            await YarnConnection.Instance.BreakConnection();
            await ClothsController.Instance.AddNewClothAndShiftRight();
            SpoolController.Instance.RemoveSpool((Spool)spool).Forget();
            return;
        }

        Yarn topYarn = spool.GetTopYarn();
        IFillable match = await FindYarnMatch(topYarn.Data);
        if (match != null)
        {
            if (match != latestFillable)
            {
                latestFillable = match;
                await YarnConnection.Instance.BreakConnection();
            }
            YarnConnection.Instance.SetConnectionPoints(topYarn, match.Connector);
            YarnConnection.Instance.ActivateConnection(topYarn.Data).Forget();
            var duration = match.FillDuration;
            var fill = match.Fill(topYarn.Data);
            var unroll = spool.UnrollTopYarn(duration);
            await UniTask.WhenAll(unroll, fill);
        }
        else
        {
            // There is no available deposit.
            await YarnConnection.Instance.BreakConnection();
            // GameManager.CurrentState.Value = GameState.Playing;
            return;
        }

        await EmptySpool(spool);
    }
    public async UniTask<IFillable> FindYarnMatch(YarnData data)
    {
        // First, we need to check cloths.
        ClothPart clothPart = ClothsController.Instance.GetClothWithData(data);
        if (clothPart != null) return clothPart;

        // Then check deposits.
        if (DepositSpoolController.Instance.HasEmptyDepositSpool)
        {
            return DepositSpoolController.Instance.FirstEmptyDepositSpool;
        }
        else
        {
            await YarnConnection.Instance.BreakConnection();
            await DepositSpoolController.Instance.HandleOverloadingAsync();
            return DepositSpoolController.Instance.FirstEmptyDepositSpool;
        }
    }
    public void SelectSpool(Spool clicked)
    {
        SelectedSpool.Value = clicked;
    }
}
