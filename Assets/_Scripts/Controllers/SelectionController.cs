using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using R3;
using Unity.Mathematics;
using UnityEngine;

public class SelectionController : MonoSingleton<SelectionController>
{
    public DepositSpool[] FillerSpools;
    public ReactiveProperty<Spool> SelectedSpool = new ReactiveProperty<Spool>();

    private void Start()
    {
        SelectedSpool
        .Where(spool => spool != null)
        .Subscribe(
            async spool =>
            {
                Debug.Log($"Selected {spool.gameObject.name}");
                await EmptySpool(spool);
            }
        )
        .AddTo(this);
    }
    IFillable latestFillable;
    public async UniTask EmptySpool(BaseSpool spool)
    {
        if (spool.IsEmpty)
        {
            // await YarnConnection.Instance.BreakConnection();
            Debug.Log(" There is no available deposit. Destroy leftovers.");
            return;
        }

        Yarn topYarn = spool.GetTopYarn();
        IFillable match = FindYarnMatch(topYarn.Data);
        if (match != null)
        {
            if (match != latestFillable)
            {
                latestFillable = match;
                // await YarnConnection.Instance.BreakConnection();
            }
            YarnConnection.Instance.SetConnectionPoints(topYarn, match.Connector);
            var setConnection = YarnConnection.Instance.ActivateConnection(topYarn.Data);
            var unrollTop = spool.UnrollTopYarn();
            // await setConnection;
            var filling = match.Fill(topYarn.Data);
            // await UniTask.WhenAll(filling, unrollTop);
            await filling;
            await YarnConnection.Instance.BreakConnection();
        }
        else
        {
            // await YarnConnection.Instance.BreakConnection();
            // There is no available deposit. destroy this.
            return;
        }

        await EmptySpool(spool);
    }
    public IFillable FindYarnMatch(YarnData data)
    {
        // First, we need to check cloths.
        ClothPart clothPart = ClothsController.Instance.GetClothWithData(data);
        if (clothPart != null) return clothPart;

        // Then check deposits.
        if (DepositSpoolController.Instance.HasEmptyDepositSpool)
        {
            Debug.Log("There are empty deposits!");
            return DepositSpoolController.Instance.FirstEmptyDepositSpool;
        }

        return null;
    }
    public void SelectSpool(Spool clicked)
    {
        SelectedSpool.Value = clicked;
    }
}
