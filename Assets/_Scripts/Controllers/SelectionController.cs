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
    public async UniTask EmptySpool(BaseSpool spool)
    {
        Yarn topYarn = spool.GetTopYarn();
        if (topYarn == null)
        {
            await HandleEmptySpool(spool);
            return;
        }

        // Check if continuing with the previous cloth or starting a new cloth
        ClothPart clothPart = ClothsController.Instance.GetClothWithData(topYarn.Data);
        if (clothPart != null && latestFillable is ClothPart && Equals(latestFillable, clothPart))
        {
            // Continue knitting the previous cloth with the same yarn type
        }
        else
        {
            await StartNewCloth();
        }

        // Handle game end states
        if (GameManager.CurrentState.Value == GameState.Victory || GameManager.CurrentState.Value == GameState.GameOver)
        {
            await YarnConnection.Instance.BreakConnection();
            return;
        }

        GameManager.CurrentState.Value = GameState.InProgress;

        // Find matching cloth or deposit spool for the yarn
        IFillable match = await FindYarnMatch(topYarn.Data);
        if (match == null)
        {
            await YarnConnection.Instance.BreakConnection();
            return;
        }

        await ProcessYarnMatch(spool, topYarn, match);
        await EmptySpool(spool); // Continue emptying the spool recursively
    }

    private async UniTask HandleEmptySpool(BaseSpool spool)
    {
        await YarnConnection.Instance.BreakConnection();
        await ClothsController.Instance.AddNewClothAndShiftRight();
        SpoolController.Instance.RemoveSpool((Spool)spool).Forget();
    }

    private async UniTask StartNewCloth()
    {
        await fill;
        await YarnConnection.Instance.BreakConnection();
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

    public async UniTask<IFillable> FindYarnMatch(YarnData data)
    {
        // First, check for a matching cloth part
        ClothPart clothPart = ClothsController.Instance.GetClothWithData(data);
        if (clothPart != null) return clothPart;

        // Then, check for an available deposit spool
        if (DepositSpoolController.Instance.HasEmptyDepositSpool)
        {
            return DepositSpoolController.Instance.FirstEmptyDepositSpool;
        }

        // Handle overloading if no deposit spool is available
        await YarnConnection.Instance.BreakConnection();
        await DepositSpoolController.Instance.HandleOverloadingAsync();
        return DepositSpoolController.Instance.FirstEmptyDepositSpool;
    }

    public void SelectSpool(Spool clicked)
    {
        SelectedSpool.Value = clicked;
    }
}
