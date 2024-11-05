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
    UniTask fill;
    ClothPart clothPart;
    public async UniTask EmptySpool(BaseSpool spool)
    {
        Yarn topYarn = spool.GetTopYarn();

        // Spool cleared. Break process.
        if (topYarn == null)
        {
            await YarnConnection.Instance.BreakConnection();

            await fill;
            fill = UniTask.CompletedTask;

            SpoolController.Instance.RemoveSpool((Spool)spool).Forget();
            await ClothsController.Instance.AddNewClothAndShiftRight();
            return;
        }

        // Need to check if we keep knitting same cloth.
        // To do this:

        // Find current cloth match.
        clothPart = ClothsController.Instance.GetClothWithData(topYarn.Data);

        // This is the condition that checks if there is a cloth match and it is the same cloth with the previous cloth.
        bool continueKnitting = clothPart && latestFillable is ClothPart && latestFillable == (IFillable)clothPart;

        // If we don't continue with same color, break the connection and wait previous cloth part to complete.
        if (!continueKnitting)
        {
            await fill;
            await YarnConnection.Instance.BreakConnection();
        }

        // If we completed every possible cloth or somehow failed, break the process.
        if (GameManager.CurrentState.Value == GameState.Victory || GameManager.CurrentState.Value == GameState.GameOver)
        {
            await YarnConnection.Instance.BreakConnection();
            return;
        }

        // If we got here, it means we have some yarns to knit clothes.
        // Set the current state to in progress.
        GameManager.CurrentState.Value = GameState.InProgress;

        // Avoid recalculations and find match by checking if we found cloth above,
        // if not, get an empty deposit.
        IFillable match = clothPart ?? await GetDepositMatch();

        // Empty clothpart to avoid false comparisons recursively.
        clothPart = null;

        // Since this match will be filled surely, we can assign it as
        // the latest fillable for later comparisons.
        if (match != latestFillable)
        {
            latestFillable = match;
            await YarnConnection.Instance.BreakConnection();
        }

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
        await EmptySpool(spool);
    }
    public async UniTask<IFillable> FindYarnMatch(YarnData data)
    {
        // First, we need to check cloths.
        ClothPart clothPart = ClothsController.Instance.GetClothWithData(data);
        if (clothPart != null) return clothPart;
        // Then check deposits.
        return await GetDepositMatch();
    }
    public async UniTask<IFillable> GetDepositMatch()
    {
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
    private async UniTask HandleEmptySpool(BaseSpool spool)
    {
        await YarnConnection.Instance.BreakConnection();
        await fill;
        fill = UniTask.CompletedTask;
        await ClothsController.Instance.AddNewClothAndShiftRight();
        SpoolController.Instance.RemoveSpool((Spool)spool).Forget();
    }

    private async UniTask StartNewCloth()
    {
        await YarnConnection.Instance.BreakConnection();
        // await fill;
        // fill = UniTask.CompletedTask;
    }

    private async UniTask ProcessYarnMatch(BaseSpool spool, Yarn topYarn, IFillable match)
    {
        if (match != latestFillable)
        {
            latestFillable = match;
            await YarnConnection.Instance.BreakConnection();
        }

        // Set up and activate connection
        YarnConnection.Instance.SetConnectionPoints(topYarn, match.Connector);
        YarnConnection.Instance.ActivateConnection(topYarn.Data).Forget();

        // Start unrolling and filling processes
        var duration = match.FillDuration;
        fill = match.Fill(topYarn.Data);
        var unroll = spool.UnrollTopYarn(duration);

        await unroll;
    }

    // public async UniTask<IFillable> FindYarnMatch(YarnData data)
    // {
    //     // First, check for a matching cloth part
    //     ClothPart clothPart = ClothsController.Instance.GetClothWithData(data);
    //     if (clothPart != null) return clothPart;

    //     // Then, check for an available deposit spool
    //     if (DepositSpoolController.Instance.HasEmptyDepositSpool)
    //     {
    //         return DepositSpoolController.Instance.FirstEmptyDepositSpool;
    //     }

    //     // Handle overloading if no deposit spool is available
    //     await YarnConnection.Instance.BreakConnection();
    //     await DepositSpoolController.Instance.HandleOverloadingAsync();
    //     return DepositSpoolController.Instance.FirstEmptyDepositSpool;
    // }

    public void SelectSpool(Spool clicked)
    {
        SelectedSpool.Value = clicked;
    }
}
